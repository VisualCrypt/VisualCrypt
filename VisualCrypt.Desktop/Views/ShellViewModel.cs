using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.Events;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.PrismSupport;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.Views
{
	[Export]
	public class ShellViewModel : ViewModelBase
	{
		readonly IEventAggregator _eventAggregator;
		readonly ILoggerFacade _logger;
		readonly IMessageBoxService _messageBoxService;
		readonly IEncryptionService _encryptionService;
		LongRunningOperation _longRunningOperation;

		[ImportingConstructor]
		public ShellViewModel(IEventAggregator eventAggregator, ILoggerFacade logger,
			IMessageBoxService messageBoxService, IEncryptionService encryptionService)
		{

			_eventAggregator = eventAggregator;
			_logger = logger;
			_messageBoxService = messageBoxService;
			_encryptionService = encryptionService;
			_eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Subscribe(OnEditorSendsStatusBarInfo);
			_eventAggregator.GetEvent<EditorSendsText>().Subscribe(ExecuteEditorSendsTextCallback);
		}


		public void OpenFromCommandLineOrNew()
		{
			var args = Environment.GetCommandLineArgs();
			if (args.Length == 2 && !string.IsNullOrWhiteSpace(args[1]))
			{
				var fileName = args[1];
				_logger.Log("Loading file from Commandline: {0}".FormatInvariant(fileName), Category.Info,
					Priority.None);
				OpenFileCommon(fileName);
			}
			else
			{
				ExecuteNewCommand();
				_logger.Log("Started with new file - Ready.", Category.Info, Priority.None);
			}
		}
		void ExecuteEditorSendsTextCallback(EditorSendsText args)
		{
			if (args != null && args.Callback != null)
				args.Callback(args.Text);
		}

		async Task<string> EditorSendsTextAsync()
		{
			var tcs = new TaskCompletionSource<string>();
			_eventAggregator.GetEvent<EditorShouldSendText>()
				.Publish(tcs.SetResult);
			return await tcs.Task;
		}

		void OnEditorSendsStatusBarInfo(string statusBarInfo)
		{
			StatusBarText = statusBarInfo;
		}

		#region INPC Properties

		public string StatusBarText
		{
			get { return _statusBarText; }
			set
			{
				if (_statusBarText == value) return;
				_statusBarText = value;
				OnPropertyChanged(() => StatusBarText);
			}
		}

		string _statusBarText;



		#endregion

		#region ExitCommand

		public DelegateCommand<CancelEventArgs> ExitCommand
		{
			get { return CreateCommand(ref _exitCommand, ExecuteExitCommand, e => true); }
		}

		DelegateCommand<CancelEventArgs> _exitCommand;

		bool _isExitConfirmed;

		void ExecuteExitCommand(CancelEventArgs e)
		{
			bool isInvokedFromWindowCloseEvent = e != null;

			if (isInvokedFromWindowCloseEvent)
			{
				if (_isExitConfirmed)
					return;
				if (ConfirmToDiscardText())
					return;
				e.Cancel = true;
			}
			else
			{
				if (ConfirmToDiscardText())
				{
					_isExitConfirmed = true;
					Application.Current.Shutdown();
				}
			}
		}

		#endregion

		#region NewCommand

		DelegateCommand _newCommand;

		public DelegateCommand NewCommand
		{
			get { return CreateCommand(ref _newCommand, ExecuteNewCommand, () => true); }
		}

		void ExecuteNewCommand()
		{
			if (!ConfirmToDiscardText())
				return;
			FileManager.FileModel = FileModel.EmptyCleartext();
			_eventAggregator.GetEvent<EditorReceivesText>().Publish(FileManager.FileModel.ClearTextContents);
		}

		#endregion

		#region ImportWithEncodingCommand

		DelegateCommand _importWithEncodingCommand;

		public DelegateCommand ImportWithEncodingCommand
		{
			get { return CreateCommand(ref _importWithEncodingCommand, ExecuteImportWithEncodingCommand, () => true); }
		}

		async void ExecuteImportWithEncodingCommand()
		{
			try
			{
				if (!ConfirmToDiscardText())
					return;

				var importEncoding = await WindowManager.GetDialogFromShowDialogAsyncWhenClosed<ImportEncodingDialog>();


				if (importEncoding.DialogResult == true)
				{
					if (!ConfirmToDiscardText())
						return;

					var selectedEncoding = importEncoding.SelectedEncodingInfo.GetEncoding();

					string title = "Import With Encoding: {0})".FormatInvariant(selectedEncoding);

					var pickFileResult = await PickFileAsync(null, DialogFilter.Text, DialogDirection.Open, title);
					if (pickFileResult.Item1)
					{
						string filename = pickFileResult.Item2;
						string importedString = File.ReadAllText(filename, selectedEncoding);
						FileManager.FileModel = FileModel.Cleartext(filename, importedString, selectedEncoding);
						_eventAggregator.GetEvent<EditorReceivesText>().Publish(FileManager.FileModel.ClearTextContents);
					}
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region ExportCommand

		DelegateCommand _exportCommand;

		public DelegateCommand ExportCommand
		{
			get { return CreateCommand(ref _exportCommand, ExecuteExportCommand, () => !FileManager.FileModel.IsEncrypted && FileManager.FileModel.SaveEncoding != null); }
		}

		async void ExecuteExportCommand()
		{
			try
			{
				var editorClearText = await EditorSendsTextAsync();

				if (editorClearText == null)
					throw new InvalidOperationException("The text received from the editor was null.");

				string title = "Export Clear Text (Encoding: {0})".FormatInvariant(
					FileManager.FileModel.SaveEncoding.EncodingName);

				string suggestedFilename = FileManager.FileModel.Filename.ReplaceCaseInsensitive(Constants.DotVisualCrypt,
					string.Empty);
				var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.Text, DialogDirection.Save, title);
				if (pickFileResult.Item1)
				{
					byte[] encodedTextBytes = FileManager.FileModel.SaveEncoding.GetBytes(editorClearText);
					File.WriteAllBytes(pickFileResult.Item2, encodedTextBytes);
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region HelpCommand

		DelegateCommand _helpCommand;

		public DelegateCommand HelpCommand
		{
			get { return CreateCommand(ref _helpCommand, ExecuteHelpCommand, () => true); }
		}

		void ExecuteHelpCommand()
		{
			try
			{
				using (var process = new Process { StartInfo = { UseShellExecute = true, FileName = Constants.HelpUrl } })
					process.Start();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region AboutCommand

		public DelegateCommand AboutCommand
		{
			get { return CreateCommand(ref _aboutCommand, ExecuteAboutCommand, () => true); }
		}

		DelegateCommand _aboutCommand;

		async void ExecuteAboutCommand()
		{
			try
			{
				await WindowManager.GetDialogFromShowDialogAsyncWhenClosed<AboutDialog>();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region LogCommand

		public DelegateCommand LogCommand
		{
			get { return CreateCommand(ref _logCommand, ExecuteLogCommand, () => true); }
		}


		DelegateCommand _logCommand;


		async void ExecuteLogCommand()
		{
			try
			{
				await WindowManager.ShowWindowAsyncAndWaitForClose<LogWindow>();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region OpenCommand

		DelegateCommand _openCommand;

		public DelegateCommand OpenCommand
		{
			get { return CreateCommand(ref _openCommand, ExecuteOpenCommand, () => true); }
		}

		async void ExecuteOpenCommand()
		{
			if (!ConfirmToDiscardText())
				return;

			var pickFileResult = await PickFileAsync(null, DialogFilter.VisualCrypt, DialogDirection.Open);
			if (pickFileResult.Item1)
			{
				OpenFileCommon(pickFileResult.Item2);
			}
		}


		async void OpenFileCommon(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
				return;

			try
			{
				var openFileResponse = _encryptionService.OpenFile(filename);
				if (!openFileResponse.IsSuccess)
				{
					_messageBoxService.ShowError(openFileResponse.Error);
					return;
				}
				if (openFileResponse.Result.SaveEncoding == null)
				{
					if (_messageBoxService.Show("This file is neither text nor VisualCrypt - display with Hex View?\r\n\r\n" +
						"If file is very large the editor may become less responsive.", "Binary File",
						MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
						return;
				}
				FileManager.FileModel = openFileResponse.Result;
				SettingsManager.CurrentDirectoryName = Path.GetDirectoryName(filename);

				var ert = _eventAggregator.GetEvent<EditorReceivesText>();
				var text = FileManager.FileModel.IsEncrypted
						? FileManager.FileModel.VisualCryptText
						: FileManager.FileModel.ClearTextContents;
				ert.Publish(text);


				// if the loaded file was cleartext we are all done
				if (!FileManager.FileModel.IsEncrypted)
					return;

				// if it's encrypted, check if we have SOME password
				if (!PasswordManager.PasswordInfo.IsPasswordSet)
				{
					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecryptLoadedFile);
					if (result == false)
						return; // The user prefers to look at the cipher!
				}

			tryDecryptLoadFileWithCurrentPassword:

				// We have a password, but we don't know if it's the right one. We must try!
				using (_longRunningOperation = StartLongRunnungOperation("Trying to decrypt opened file"))
				{
					var decryptForDisplayResult = await Task.Run(() => _encryptionService.DecryptForDisplay(FileManager.FileModel,
				FileManager.FileModel.VisualCryptText, _longRunningOperation.Context));
					if (decryptForDisplayResult.IsSuccess)
					{
						// we were lucky, the password we have is correct!
						FileManager.FileModel = decryptForDisplayResult.Result; // do this before pushing the text to the editor
						_eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
						UpdateCanExecuteChanged();
						return; // exit from this goto-loop, we have a new decrypted file
					}
					if (decryptForDisplayResult.IsCancelled)
					{
						return; // we also exit from whole procedure (we could also move on to redisplay 
						// the password entry, as below, in case of error, which we interpret as wrong password).
					}
					FileManager.ShowEncryptedBar(); // Error, i.e. wrong password, show EncryptedBar
				}

				// As we tested that it's valid VisualCrypt in the open routine, we can assume we are here because
				// the password was wrong. So we ask the user again..:
				bool result2 = await SetPasswordAsync(SetPasswordDialogMode.CorrectPassword);
				if (result2 == false)
					return; // The user prefers to look at the cipher!
				// We have another password, from the user, we try again!
				goto tryDecryptLoadFileWithCurrentPassword;
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		public void OpenFileFromDragDrop(string dropFilename)
		{
			if (!ConfirmToDiscardText())
				return;
			OpenFileCommon(dropFilename);
		}

		#endregion

		#region EncryptEditorContentsCommand

		DelegateCommand _encryptEditorContentsCommand;

		public DelegateCommand EncryptEditorContentsCommand
		{
			get
			{
				return CreateCommand(ref _encryptEditorContentsCommand, ExecuteEncryptEditorContentsCommand,
					() => !FileManager.FileModel.IsEncrypted && FileManager.FileModel.SaveEncoding != null);
			}
		}


		async void ExecuteEncryptEditorContentsCommand()
		{
			try
			{
				if (!PasswordManager.PasswordInfo.IsPasswordSet)
				{
					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncrypt);
					if (result == false)
						return;
				}

				string textBufferContents = await EditorSendsTextAsync();

				using (_longRunningOperation = StartLongRunnungOperation("Encrypting"))
				{
					var createEncryptedFileResponse =
					await Task.Run(() => _encryptionService.EncryptForDisplay(FileManager.FileModel, textBufferContents, _longRunningOperation.Context));
					if (createEncryptedFileResponse.IsSuccess)
					{
						FileManager.FileModel = createEncryptedFileResponse.Result; // do this before pushing the text to the editor
						_eventAggregator.GetEvent<EditorReceivesText>().Publish(createEncryptedFileResponse.Result.VisualCryptText);
						UpdateCanExecuteChanged();
						return;
					}
					if (createEncryptedFileResponse.IsCancelled)
						return;
					// other error, switch back to PlainTextBar and show error
					FileManager.ShowPlainTextBar();
					_messageBoxService.ShowError(createEncryptedFileResponse.Error);
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region DecryptEditorContentsCommand

		DelegateCommand _decryptEditorContentsCommand;

		public DelegateCommand DecryptEditorContentsCommand
		{
			get
			{
				return CreateCommand(ref _decryptEditorContentsCommand, ExecuteDecryptEditorContentsCommand,
					() => FileManager.FileModel.SaveEncoding != null);
			}
		}


		async void ExecuteDecryptEditorContentsCommand()
		{
			try
			{
				if (!PasswordManager.PasswordInfo.IsPasswordSet)
				{
					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecrypt);
					if (result == false)
						return;
				}

				string textBufferContents = await EditorSendsTextAsync();

				using (_longRunningOperation = StartLongRunnungOperation("Decryption"))
				{
					var decryptForDisplayResult =
					await Task.Run(() => _encryptionService.DecryptForDisplay(FileManager.FileModel, textBufferContents, _longRunningOperation.Context));
					if (decryptForDisplayResult.IsSuccess)
					{
						FileManager.FileModel = decryptForDisplayResult.Result; // do this before pushing the text to the editor
						_eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
						UpdateCanExecuteChanged();
						return;
					}
					if (decryptForDisplayResult.IsCancelled)
						return;
					// other error, switch back to EncryptedBar
					FileManager.ShowEncryptedBar();
					_messageBoxService.ShowError(decryptForDisplayResult.Error);
				}

			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		LongRunningOperation StartLongRunnungOperation(string description)
		{
			FileManager.ShowWorkingBar(description);

			Action<int> updateProgressBar = i => FileManager.BindableFileInfo.ProgressPercent = i;

			var switchBackToPreviousBar = FileManager.FileModel.IsEncrypted
				? (Action) FileManager.ShowEncryptedBar
				: FileManager.ShowPlainTextBar;

			return new LongRunningOperation(updateProgressBar, switchBackToPreviousBar);
		}



		#endregion

		#region SaveCommand

		DelegateCommand _saveCommand;

		public DelegateCommand SaveCommand
		{
			get { return CreateCommand(ref _saveCommand, ExecuteSaveCommand, () => FileManager.FileModel.SaveEncoding != null); }
		}

		void ExecuteSaveCommand()
		{
			ExecuteSaveCommandsCommon(false);
		}

		async void ExecuteSaveCommandsCommon(bool isSaveAs)
		{
			try
			{
				// This is the simple case, we can 'just save'.
				if (FileManager.FileModel.IsEncrypted && !isSaveAs && FileManager.FileModel.CheckFilenameForQuickSave())
				{
					var response = _encryptionService.SaveEncryptedFile(FileManager.FileModel);
					if (!response.IsSuccess)
						throw new Exception(response.Error);
					FileManager.FileModel.IsDirty = false;
				}
				// This is the case where we need a new filename and can then also 'just save'.
				else if (FileManager.FileModel.IsEncrypted && (isSaveAs || !FileManager.FileModel.CheckFilenameForQuickSave()))
				{
					string suggestedFilename = null;
					if (isSaveAs)
						suggestedFilename = FileManager.FileModel.Filename;

					var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
					if (pickFileResult.Item1)
					{
						FileManager.FileModel.Filename = pickFileResult.Item2;
						var response = _encryptionService.SaveEncryptedFile(FileManager.FileModel);
						if (!response.IsSuccess)
							throw new Exception(response.Error);
						FileManager.FileModel.IsDirty = false;
					}
				}
				// And in that case we need a different strategy: Encrypt and THEN save.
				else
				{
					if (FileManager.FileModel.IsEncrypted)
						throw new InvalidOperationException("We assert confusion!");
					await EncryptAndThenSave(isSaveAs);
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		async Task EncryptAndThenSave(bool isSaveAs)
		{
			// We have been called from Save/SaveAs because the file is not encrypted yet.
			// We are still in a try/catch block

			// To encrypt and then save, we must sure we have a password:
			if (!PasswordManager.PasswordInfo.IsPasswordSet)
			{
				bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncryptAndSave);
				if (result == false)
					return;
			}
			// Then we must sure we have the file name:
			if (isSaveAs || !FileManager.FileModel.CheckFilenameForQuickSave())
			{
				string suggestedFilename = null;
				if (isSaveAs)
					suggestedFilename = FileManager.FileModel.Filename;

				var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
				if (!pickFileResult.Item1)
					return;
				FileManager.FileModel.Filename = pickFileResult.Item2;
			}
			// No we have password and filename, we can now encrypt and save in one step.
			// We will not replace FileManager.FileModel because we continue editing the same cleartext.
			string editorClearText = await EditorSendsTextAsync();

			using (_longRunningOperation = StartLongRunnungOperation("Encrypt and then save."))
			{
				var encryptAndSaveFileResponse =
				await Task.Run(() => _encryptionService.EncryptAndSaveFile(FileManager.FileModel, editorClearText, _longRunningOperation.Context));

				if (encryptAndSaveFileResponse.IsSuccess)
				{
					string visualCryptTextSaved = encryptAndSaveFileResponse.Result;
					// We are done with successful saving. Show that we did encrypt the text:
					_eventAggregator.GetEvent<EditorReceivesText>().Publish(visualCryptTextSaved);
					FileManager.ShowEncryptedBar();
					await Task.Delay(2000);
					FileManager.ShowPlainTextBar();
					// Redisplay the clear text, to allow continue editing.
					FileManager.FileModel.IsDirty = false;

					_eventAggregator.GetEvent<EditorReceivesText>().Publish(editorClearText);

					UpdateCanExecuteChanged();
					return;
				}
				if (encryptAndSaveFileResponse.IsCancelled)
				{
					return; // Cancel means the user can continue editing clear text
				}
				// other error, switch back to PlainTextBar
				FileManager.ShowPlainTextBar();
				throw new Exception(encryptAndSaveFileResponse.Error);
			}




		}

		#endregion

		#region SaveAsCommand

		DelegateCommand _saveAsCommand;

		public DelegateCommand SaveAsCommand
		{
			get { return CreateCommand(ref _saveAsCommand, ExecuteSaveAsCommand, () => FileManager.FileModel.SaveEncoding != null); }
		}

		void ExecuteSaveAsCommand()
		{
			ExecuteSaveCommandsCommon(true);
		}

		#endregion

		#region ShowSetPasswordDialogCommand

		DelegateCommand _showSetPasswordDialogCommand;

		public DelegateCommand ShowSetPasswordDialogCommand
		{
			get
			{
				if (_showSetPasswordDialogCommand == null)
					_showSetPasswordDialogCommand = DelegateCommand.FromAsyncHandler(ExecuteShowSetPasswordDialogCommand, () => true);
				return _showSetPasswordDialogCommand;
			}
		}

		async Task ExecuteShowSetPasswordDialogCommand()
		{
			try
			{
				if (PasswordManager.PasswordInfo.IsPasswordSet)
					await SetPasswordAsync(SetPasswordDialogMode.Change);
				else
				{
					await SetPasswordAsync(SetPasswordDialogMode.Set);
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}


		/// <summary>
		/// Returns true, if the user did positively set a password, otherwise always false;
		/// </summary>
		async Task<bool> SetPasswordAsync(SetPasswordDialogMode setPasswordDialogMode)
		{
			ParamsProvider.SetParams(typeof(SetPasswordDialog), setPasswordDialogMode);
			return await WindowManager.GetBoolFromShowDialogAsyncWhenClosed<SetPasswordDialog>();
		}

		#endregion

		#region ShowSettingsDialogCommand

		DelegateCommand _showSettingsDialogCommand;

		public DelegateCommand ShowSettingsDialogCommand
		{
			get
			{
				if (_showSettingsDialogCommand == null)
					_showSetPasswordDialogCommand = DelegateCommand.FromAsyncHandler(ExecuteShowSettingsDialogCommand, () => true);
				return _showSetPasswordDialogCommand;
			}
		}

		async Task ExecuteShowSettingsDialogCommand()
		{
			try
			{
			
				var result =  await WindowManager.GetBoolFromShowDialogAsyncWhenClosed<SettingsDialog>();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}



		#endregion

		#region ClearPasswordCommand

		public DelegateCommand ClearPasswordCommand
		{
			get { return CreateCommand(ref _clearPasswordCommand, ExecuteClearPasswordCommand, () => true); }
		}

		DelegateCommand _clearPasswordCommand;

		void ExecuteClearPasswordCommand()
		{
			try
			{
				var clearPasswordResponse = _encryptionService.ClearPassword();
				if (!clearPasswordResponse.IsSuccess)
				{
					_messageBoxService.ShowError(clearPasswordResponse.Error);
					return;
				}
				PasswordManager.PasswordInfo.IsPasswordSet = false;
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e.Message);
			}
		}

		#endregion

		#region CopyAllCommand
		public DelegateCommand CopyAllCommand
		{
			get { return CreateCommand(ref _copyAllCommand, ExecuteCopyAllCommand, () => true); }
		}

		DelegateCommand _copyAllCommand;

		void ExecuteCopyAllCommand()
		{
			try
			{
				var cipherText = FileManager.FileModel.VisualCryptText;
				Clipboard.SetText(cipherText, TextDataFormat.Text);
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e.Message);
			}
		}
		#endregion

		#region private methods

		void UpdateCanExecuteChanged()
		{
			// TODO: many more Commands, like Replace must be updated
			_exportCommand.RaiseCanExecuteChanged();
			_encryptEditorContentsCommand.RaiseCanExecuteChanged();
			_decryptEditorContentsCommand.RaiseCanExecuteChanged();
		}

		bool ConfirmToDiscardText()
		{
			if (FileManager.FileModel.IsDirty)
				return
					(_messageBoxService.Show("Discard changes?", Constants.ProductName, MessageBoxButton.OKCancel,
						MessageBoxImage.Question) ==
					 MessageBoxResult.OK);

			return true;
		}

		static async Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter,
			DialogDirection dialogDirection, string title = null)
		{
			FileDialog fileDialog;
			if (dialogDirection == DialogDirection.Open)
				fileDialog = new OpenFileDialog();
			else
				fileDialog = new SaveFileDialog();

			if (title != null)
				fileDialog.Title = title;

			if (!string.IsNullOrEmpty(suggestedFilename))
				fileDialog.FileName = suggestedFilename;

			fileDialog.InitialDirectory = SettingsManager.CurrentDirectoryName;
			if (diaglogFilter == DialogFilter.VisualCrypt)
			{
				fileDialog.DefaultExt = Constants.VisualCryptDialogFilter_DefaultExt;
				fileDialog.Filter = Constants.VisualCryptDialogFilter;
			}
			else
			{
				fileDialog.DefaultExt = Constants.TextDialogFilter_DefaultExt;
				fileDialog.Filter = Constants.TextDialogFilter;
			}

			var tcs = new TaskCompletionSource<Tuple<bool, string>>();

			var okClicked = fileDialog.ShowDialog() == true;
			var selectedFilename = fileDialog.FileName;

			tcs.SetResult(new Tuple<bool, string>(okClicked, selectedFilename));
			return await tcs.Task;
		}

		#endregion

		public void CancelLongRunningOperation()
		{
			_longRunningOperation.Cancel();
		}
	}
}