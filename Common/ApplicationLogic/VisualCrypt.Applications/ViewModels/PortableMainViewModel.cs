using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Events;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Language;

namespace VisualCrypt.Applications.ViewModels
{
    public class PortableMainViewModel : ViewModelBase, IActiveCleanup, IEditorContext
	{
		public PasswordInfo PasswordInfo
		{
			get { return _passwordInfo; }
		}

		readonly PasswordInfo _passwordInfo = new PasswordInfo();

		public StatusBarModel StatusBarModel
		{
			get { return _statusBarModel; }
		}
		readonly StatusBarModel _statusBarModel = new StatusBarModel();

		public FileModel FileModel
		{
			get { return _fileModel; }
		}
		readonly FileModel _fileModel;

		IFileModel IEditorContext.FileModel
		{
			get { return FileModel; }
		}

        #region Bound Properties

        public ISettingsManager SettingsManager
        {
            get { return _settingsManager; }
        }

        #endregion

        readonly ILog _log;
        readonly IEventAggregator _eventAggregator;
		readonly IMessageBoxService _messageBoxService;
		readonly IEncryptionService _encryptionService;
		public readonly INavigationService NavigationService;
		readonly IPasswordDialogDispatcher _passwordDialogDispatcher;
		readonly ISettingsManager _settingsManager;
		readonly IFileService _fileService;
		readonly IBrowserService _browserService;
		readonly IAssemblyInfoProvider _assemblyInfoProvider;
		readonly ILifeTimeService _lifeTimeService;
		readonly IClipBoardService _clipBoardService;
		readonly IWindowManager _windowManager;

		LongRunningOperation _longRunningOperation;

		public PortableMainViewModel()
		{
			
			_log = Service.Get<ILog>();
            _eventAggregator = Service.Get<IEventAggregator>();

            _messageBoxService = Service.Get<IMessageBoxService>();
            _encryptionService = Service.Get<IEncryptionService>();

            NavigationService = Service.Get<INavigationService>();
            _passwordDialogDispatcher = Service.Get<IPasswordDialogDispatcher>();


            _settingsManager = Service.Get<ISettingsManager>();
            _fileService = Service.Get<IFileService>();

            _browserService = Service.Get<IBrowserService>();
            _assemblyInfoProvider = Service.Get<IAssemblyInfoProvider>();

            _lifeTimeService = Service.Get<ILifeTimeService>();
            _clipBoardService = Service.Get<IClipBoardService>();

            _windowManager = Service.Get<IWindowManager>();

            _fileModel = FileModel.EmptyCleartext();
			_fileModel.OnFileModelUpdated = (fileModel) =>
			{
				StatusBarModel.OnFileModelChanged(fileModel);
				_eventAggregator.GetEvent<FileModelChanged>().Publish(fileModel);
			};
			_eventAggregator.GetEvent<EditorSendsText>().Subscribe(ExecuteEditorSendsTextCallback);
			_eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Subscribe(StatusBarModel.UpdateStatusBarText);
		}

	


		public async Task OnNavigatedToCompletedAndLoaded(FilesPageCommandArgs command)
		{
			Debug.Assert(command != null);

			switch (command.FilesPageCommand)
			{
				case FilesPageCommand.New:
					ExecuteNewCommand();
					break;
				case FilesPageCommand.Open:
					await OpenFileCommon(command.FileReference.Filename);
					break;
				default:
					throw new InvalidOperationException(string.Format("Unknwon command {0}", command.FilesPageCommand));
			}
		}

		public void OpenFromCommandLineOrNew(string[] args)
		{
			if (args.Length == 2 && !string.IsNullOrWhiteSpace(args[1]))
			{
				var fileName = args[1];
				_log.Debug(string.Format(CultureInfo.InvariantCulture, "Loading file from Commandline: {0}", fileName));

				OpenFileCommon(fileName);
			}
			else
			{
				ExecuteNewCommand();
				_log.Debug("Started with new file - Ready.");
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

		

	

		#region ExitCommand

		public DelegateCommand<CancelEventArgs> ExitCommand
		{
			get { return CreateCommand(ref _exitCommand, ExecuteExitCommand, e => true); }
		}

		DelegateCommand<CancelEventArgs> _exitCommand;



		void ExecuteExitCommand(CancelEventArgs e)
		{
			_lifeTimeService.HandleExitRequested(e, ConfirmToDiscardText);

		}

        #endregion

        #region GoBackToFilesCommand

        DelegateCommand _goBackToFilesCommand;

        public DelegateCommand GoBackToFilesCommand
        {
            get { return CreateCommand(ref _goBackToFilesCommand, ExecuteGoBackToFilesCommand, () => true); }
        }

        void ExecuteGoBackToFilesCommand()
        {
            if (!ConfirmToDiscardText())
                return;

            NavigationService.NavigateToFilesPage();
            Cleanup();
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
			FileModel.UpdateFrom(FileModel.EmptyCleartext());
			_eventAggregator.GetEvent<EditorReceivesText>().Publish(FileModel.ClearTextContents);
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

				Tuple<bool?, Encoding> importEncodingDialogResult = await _windowManager.GetDialogFromShowDialogAsyncWhenClosed_ImportEncodingDialog();

				if (importEncodingDialogResult.Item1 == true)
				{
					if (!ConfirmToDiscardText())
						return;

					var selectedEncoding = importEncodingDialogResult.Item2;

					string title = string.Format(CultureInfo.InvariantCulture, "Import With Encoding: {0})",
						selectedEncoding);

					var pickFileResult = await _fileService.PickFileAsync(null, DialogFilter.Text, DialogDirection.Open, title);
					if (pickFileResult.Item1)
					{
						string filename = pickFileResult.Item2;
						var shortFilename = Path.GetFileName(filename);
						string importedString = _fileService.ReadAllText(filename, selectedEncoding);
						FileModel.UpdateFrom(FileModel.Cleartext(filename, shortFilename, importedString,
							selectedEncoding));
						_eventAggregator.GetEvent<EditorReceivesText>().Publish(FileModel.ClearTextContents);
					}
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
				_browserService.LaunchUrl(Loc.Strings.uriHelpUrl);

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
				await _windowManager.ShowAboutDialogAsync();
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
				await _windowManager.ShowLogWindowAsync();
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

			var pickFileResult = await _fileService.PickFileAsync(null, DialogFilter.VisualCrypt, DialogDirection.Open);
			if (pickFileResult.Item1)
			{
				await OpenFileCommon(pickFileResult.Item2);
			}
		}


		async Task OpenFileCommon(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename) || !_fileService.Exists(filename))
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
					if (_messageBoxService.Show(
						"This file is neither text nor VisualCrypt - display with Hex View?\r\n\r\n" +
						"If file is very large the editor may become less responsive.", "Binary File",
						RequestButton.OKCancel, RequestImage.Warning) == RequestResult.Cancel)
						return;
				}
				FileModel.UpdateFrom(openFileResponse.Result);
				_settingsManager.CurrentDirectoryName = Path.GetDirectoryName(filename);

				var ert = _eventAggregator.GetEvent<EditorReceivesText>();
				var text = FileModel.IsEncrypted
					? FileModel.VisualCryptText
					: FileModel.ClearTextContents;
				ert.Publish(text);


				// if the loaded file was cleartext we are all done
				if (!FileModel.IsEncrypted)
					return;

				// if it's encrypted, check if we have SOME password
				if (!PasswordInfo.IsPasswordSet)
				{
					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecryptLoadedFile);
					if (result == false)
						return; // The user prefers to look at the cipher!
				}

			tryDecryptLoadFileWithCurrentPassword:

				// We have a password, but we don't know if it's the right one. We must try!
				using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationDecryptOpenedFile))
				{
					var decryptForDisplayResult =
						await Task.Run(() => _encryptionService.DecryptForDisplay(FileModel,
							FileModel.VisualCryptText, _longRunningOperation.Context));
					if (decryptForDisplayResult.IsSuccess)
					{
						// we were lucky, the password we have is correct!
						FileModel.UpdateFrom(decryptForDisplayResult.Result);
						// do this before pushing the text to the editor
						_eventAggregator.GetEvent<EditorReceivesText>()
							.Publish(decryptForDisplayResult.Result.ClearTextContents);
						UpdateCanExecuteChanged();
						return; // exit from this goto-loop, we have a new decrypted file
					}
					if (decryptForDisplayResult.IsCanceled)
					{
						return; // we also exit from whole procedure (we could also move on to redisplay 
						// the password entry, as below, in case of error, which we interpret as wrong password).
					}
					StatusBarModel.ShowEncryptedBar(); // Error, i.e. wrong password, show EncryptedBar
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
					() => !FileModel.IsEncrypted && FileModel.SaveEncoding != null);
			}
		}


		async void ExecuteEncryptEditorContentsCommand()
		{
			try
			{
				if (!PasswordInfo.IsPasswordSet)
				{
					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncrypt);
					if (result == false)
						return;
				}

				string textBufferContents = await EditorSendsTextAsync();

				using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationEncryption))
				{
					var createEncryptedFileResponse =
						await
							Task.Run(
								() =>
									_encryptionService.EncryptForDisplay(FileModel, textBufferContents,
										new RoundsExponent(_settingsManager.CryptographySettings.LogRounds),
										_longRunningOperation.Context));
					if (createEncryptedFileResponse.IsSuccess)
					{
						FileModel.UpdateFrom(createEncryptedFileResponse.Result);
						// do this before pushing the text to the editor
						_eventAggregator.GetEvent<EditorReceivesText>()
							.Publish(createEncryptedFileResponse.Result.VisualCryptText);
						UpdateCanExecuteChanged();
						return;
					}
					if (createEncryptedFileResponse.IsCanceled)
						return;
					// other error, switch back to PlainTextBar and show error
					StatusBarModel.ShowPlainTextBar();
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
					() => FileModel.SaveEncoding != null);
			}
		}


		async void ExecuteDecryptEditorContentsCommand()
		{
			try
			{
				if (!PasswordInfo.IsPasswordSet)
				{
					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecrypt);
					if (result == false)
						return;
				}

				string textBufferContents = await EditorSendsTextAsync();

				using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationDecryption))
				{
					var decryptForDisplayResult =
						await
							Task.Run(
								() =>
									_encryptionService.DecryptForDisplay(FileModel, textBufferContents,
										_longRunningOperation.Context));
					if (decryptForDisplayResult.IsSuccess)
					{
						FileModel.UpdateFrom(decryptForDisplayResult.Result);
						// do this before pushing the text to the editor
						_eventAggregator.GetEvent<EditorReceivesText>()
							.Publish(decryptForDisplayResult.Result.ClearTextContents);
						UpdateCanExecuteChanged();
						return;
					}
					if (decryptForDisplayResult.IsCanceled)
						return;
					// other error, switch back to EncryptedBar
					StatusBarModel.ShowEncryptedBar();
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
			StatusBarModel.ShowProgressBar(description);

			Action<EncryptionProgress> updateProgressBar = encryptionProgress =>
			{
				StatusBarModel.ProgressPercent = encryptionProgress.Percent;
				StatusBarModel.ProgressMessage = encryptionProgress.Message;
			};

			var switchBackToPreviousBar = FileModel.IsEncrypted
				? (Action)StatusBarModel.ShowEncryptedBar
				: StatusBarModel.ShowPlainTextBar;

			return new LongRunningOperation(updateProgressBar, switchBackToPreviousBar);
		}

		#endregion

		#region SaveCommand

		DelegateCommand _saveCommand;

		public DelegateCommand SaveCommand
		{
			get
			{
				return CreateCommand(ref _saveCommand, ExecuteSaveCommand,
					() => FileModel.SaveEncoding != null);
			}
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
				if (FileModel.IsEncrypted && !isSaveAs && _fileService.CheckFilenameForQuickSave(FileModel.Filename))
				{
					var response = _encryptionService.SaveEncryptedFile(FileModel);
					if (!response.IsSuccess)
						throw new Exception(response.Error);
					FileModel.IsDirty = false;
				}
				// This is the case where we need a new filename and can then also 'just save'.
				else if (FileModel.IsEncrypted && (isSaveAs || !_fileService.CheckFilenameForQuickSave(FileModel.Filename)))
				{
					string suggestedFilename = null;
					if (isSaveAs)
						suggestedFilename = FileModel.Filename;

					var pickFileResult =
						await _fileService.PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
					if (pickFileResult.Item1)
					{
						FileModel.Filename = pickFileResult.Item2;
						var response = _encryptionService.SaveEncryptedFile(FileModel);
						if (!response.IsSuccess)
							throw new Exception(response.Error);
						FileModel.IsDirty = false;
					}
				}
				// And in that case we need a different strategy: Encrypt and THEN save.
				else
				{
					if (FileModel.IsEncrypted)
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
			if (!PasswordInfo.IsPasswordSet)
			{
				bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncryptAndSave);
				if (result == false)
					return;
			}
			// Then we must sure we have the file name:
			if (isSaveAs || !_fileService.CheckFilenameForQuickSave(FileModel.Filename))
			{
				string suggestedFilename = null;
				if (isSaveAs)
					suggestedFilename = FileModel.Filename;

				var pickFileResult =
					await _fileService.PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
				if (!pickFileResult.Item1)
					return;
				FileModel.Filename = pickFileResult.Item2;
			}
			// No we have password and filename, we can now encrypt and save in one step.
			// We will not replace FileModel because we continue editing the same cleartext.
			string editorClearText = await EditorSendsTextAsync();

			using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationEncryptAndSave))
			{
				var encryptAndSaveFileResponse =
					await
						Task.Run(
							() =>
								_encryptionService.EncryptAndSaveFile(FileModel, editorClearText,
									new RoundsExponent(_settingsManager.CryptographySettings.LogRounds),
									_longRunningOperation.Context));

				if (encryptAndSaveFileResponse.IsSuccess)
				{
					string visualCryptTextSaved = encryptAndSaveFileResponse.Result;
					// We are done with successful saving. Show that we did encrypt the text:
					_eventAggregator.GetEvent<EditorReceivesText>().Publish(visualCryptTextSaved);
					StatusBarModel.ShowEncryptedBar();
					await Task.Delay(2000);
					StatusBarModel.ShowPlainTextBar();
					// Redisplay the clear text, to allow continue editing.
					FileModel.IsDirty = false;

					_eventAggregator.GetEvent<EditorReceivesText>().Publish(editorClearText);

					UpdateCanExecuteChanged();
					return;
				}
				if (encryptAndSaveFileResponse.IsCanceled)
				{
					return; // Cancel means the user can continue editing clear text
				}
				// other error, switch back to PlainTextBar
				StatusBarModel.ShowPlainTextBar();
				throw new Exception(encryptAndSaveFileResponse.Error);
			}
		}

		#endregion

		#region SaveAsCommand

		DelegateCommand _saveAsCommand;

		public DelegateCommand SaveAsCommand
		{
			get
			{
				return CreateCommand(ref _saveAsCommand, ExecuteSaveAsCommand,
					() => FileModel.SaveEncoding != null);
			}
		}

		void ExecuteSaveAsCommand()
		{
			ExecuteSaveCommandsCommon(true);
		}

		#endregion

		#region ExportCommand

		DelegateCommand _exportCommand;

		public DelegateCommand ExportCommand
		{
			get
			{
				return CreateCommand(ref _exportCommand, ExecuteExportCommand,
					() => !FileModel.IsEncrypted && FileModel.SaveEncoding != null);
			}
		}

		async void ExecuteExportCommand()
		{
			try
			{
				var editorClearText = await EditorSendsTextAsync();

				if (editorClearText == null)
					throw new InvalidOperationException("The text received from the editor was null.");

				string title = string.Format(CultureInfo.InvariantCulture, "Export Clear Text (Encoding: {0})",
					FileModel.SaveEncodingName);

				string suggestedFilename =
					FileModel.Filename.ReplaceCaseInsensitive(PortableConstants.DotVisualCrypt,
						string.Empty);
				var pickFileResult =
					await _fileService.PickFileAsync(suggestedFilename, DialogFilter.Text, DialogDirection.Save, title);
				if (pickFileResult.Item1)
				{
					byte[] encodedTextBytes = FileModel.SaveEncoding.GetBytes(editorClearText);
					_fileService.WriteAllBytes(pickFileResult.Item2, encodedTextBytes);
				}
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region ShowSetPasswordDialogCommand

		public DelegateCommand ShowSetPasswordDialogCommand
		{
			get
			{
				return CreateCommand(ref _showSetPasswordDialogCommand, async () => { await ExecutePasswordCommand(); }, () => true);
			}
		}
		DelegateCommand _showSetPasswordDialogCommand;


		async Task ExecutePasswordCommand()
		{
			try
			{
				if (PasswordInfo.IsPasswordSet)
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

		async Task<bool> SetPasswordAsync(SetPasswordDialogMode setPasswordDialogMode)
		{
			return
				await
					_passwordDialogDispatcher.LaunchAsync(_encryptionService, setPasswordDialogMode,
						PasswordInfo.SetIsPasswordSet, PasswordInfo.IsPasswordSet);
		}






		#endregion



		#region ShowSettingsDialogCommand

		DelegateCommand _showSettingsDialogCommand;

		public DelegateCommand ShowSettingsDialogCommand
		{
			get
			{
				return CreateCommand(ref _showSettingsDialogCommand, async () => { await ExecuteShowSettingsDialogCommand(); }, () => true);
			}
		}

		async Task ExecuteShowSettingsDialogCommand()
		{
			try
			{
				await _windowManager.GetBoolFromShowDialogAsyncWhenClosed_SettingsDialog();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}

		#endregion

		#region DefaultsCommand

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
				PasswordInfo.SetIsPasswordSet(false);
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
				_clipBoardService.CopyText(FileModel.VisualCryptText);

			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e.Message);
			}
		}

		#endregion

		#region SwitchLanguageCommand

		public DelegateCommand<string> SwitchLanguageCommand
		{
			get { return CreateCommand(ref _switchLanguageCommand, ExecuteSwitchLanguageCommand, s => true); }
		}

		DelegateCommand<string> _switchLanguageCommand;

		void ExecuteSwitchLanguageCommand(string loc)
		{
			try
			{
				Loc.SetLanguage(loc);
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

			if (FileModel.IsDirty)
				return
					(_messageBoxService.Show("Discard changes?", _assemblyInfoProvider.AssemblyProduct, RequestButton.OKCancel,
						RequestImage.Question) ==
					 RequestResult.OK);

			return true;
		}



		#endregion

		public void CancelLongRunningOperation()
		{
			_longRunningOperation.Cancel();
		}

		#region ICleanup

		public void Cleanup()
		{
            if(_longRunningOperation!= null && _longRunningOperation.Context!= null)
                _longRunningOperation.Cancel();
			_eventAggregator.GetEvent<EditorShouldCleanup>().Publish(null);
			_eventAggregator.GetEvent<EditorSendsText>().Unsubscribe(ExecuteEditorSendsTextCallback);
			_eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Unsubscribe(StatusBarModel.UpdateStatusBarText);
            FileModel.UpdateFrom(FileModel.EmptyCleartext());
		}

		#endregion
		//void ExecuteEditorSendsTextCallback(EditorSendsText args)
		//{
		//	Debug.Assert(args != null && args.Callback != null);
		//	args.Callback(args.Text);
		//}

		//Task<string> EditorSendsTextAsync()
		//{
		//	var tcs = new TaskCompletionSource<string>();
		//	_eventAggregator.GetEvent<EditorShouldSendText>()
		//		.Publish(tcs.SetResult);
		//	return tcs.Task;
		//}

		//#region NewCommand

		//void ExecuteNewCommand()
		//{
		//	if (!ConfirmToDiscardText())
		//		return;

		//	FileModel.UpdateFrom(FileModel.EmptyCleartext());

		//	var ert = _eventAggregator.GetEvent<EditorReceivesText>();
		//	ert.Publish(FileModel.ClearTextContents);
		//}

		//#endregion

		//#region GoBackToFilesCommand

		//DelegateCommand _goBackToFilesCommand;

		//public DelegateCommand GoBackToFilesCommand
		//{
		//	get { return CreateCommand(ref _goBackToFilesCommand, ExecuteGoBackToFilesCommand, () => true); }
		//}

		//void ExecuteGoBackToFilesCommand()
		//{
		//	if (!ConfirmToDiscardText())
		//		return;

		//	_navigationService.NavigateToFilesPage();
		//	Cleanup();
		//}

		//#endregion

		//#region ShowSetPasswordDialogCommand

		//public DelegateCommand ShowSetPasswordDialogCommand
		//{
		//	get
		//	{
		//		return CreateCommand(ref _showSetPasswordDialogCommand, async () => { await ExecutePasswordCommand(); }, () => true);
		//	}
		//}
		//DelegateCommand _showSetPasswordDialogCommand;


		//async Task ExecutePasswordCommand()
		//{
		//	try
		//	{
		//		if (PasswordInfo.IsPasswordSet)
		//			await SetPasswordAsync(SetPasswordDialogMode.Change);
		//		else
		//		{
		//			await SetPasswordAsync(SetPasswordDialogMode.Set);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		_messageBoxService.ShowError(e);
		//	}
		//}

		//async Task<bool> SetPasswordAsync(SetPasswordDialogMode setPasswordDialogMode)
		//{
		//	return
		//		await
		//			_passwordDialogDispatcher.LaunchAsync(_encryptionService, setPasswordDialogMode,
		//				PasswordInfo.SetIsPasswordSet,PasswordInfo.IsPasswordSet);
		//}





		//#endregion

		//#region OpenCommand

		//async Task ExecuteOpenCommand(FileReference fileReference)
		//{
		//	await OpenFileCommon(fileReference.Filename);
		//}

		//async Task OpenFileCommon(string filename)
		//{
		//	Debug.Assert(filename != null);

		//	try
		//	{
		//		var openFileResponse = _encryptionService.OpenFile(filename);
		//		if (!openFileResponse.IsSuccess)
		//		{
		//			_messageBoxService.ShowError(openFileResponse.Error);
		//			return;
		//		}
		//		if (openFileResponse.Result.SaveEncoding == null)
		//		{
		//			if (_messageBoxService.Show("This file is neither text nor VisualCrypt - display with Hex View?\r\n\r\n" +
		//										"If file is very large the editor may become less responsive.", "Binary File",
		//				RequestButton.OKCancel, RequestImage.Warning) == RequestResult.Cancel)
		//				return;
		//		}
		//		FileModel.UpdateFrom(openFileResponse.Result);
		//		_settingsManager.CurrentDirectoryName = Path.GetDirectoryName(filename);

		//		var ert = _eventAggregator.GetEvent<EditorReceivesText>();
		//		var text = FileModel.IsEncrypted
		//			? FileModel.VisualCryptText
		//			: FileModel.ClearTextContents;
		//		ert.Publish(text);


		//		// if the loaded file was cleartext we are all done
		//		if (!FileModel.IsEncrypted)
		//			return;

		//		// if it's encrypted, check if we have SOME password
		//		if (!PasswordInfo.IsPasswordSet)
		//		{
		//			bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecryptLoadedFile);
		//			if (result == false)
		//				return; // The user prefers to look at the cipher!
		//		}

		//		tryDecryptLoadFileWithCurrentPassword:

		//		// We have a password, but we don't know if it's the right one. We must try!
		//		using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationDecryptOpenedFile))
		//		{
		//			var decryptForDisplayResult = await Task.Run(() => _encryptionService.DecryptForDisplay(FileModel,
		//				FileModel.VisualCryptText, _longRunningOperation.Context));
		//			if (decryptForDisplayResult.IsSuccess)
		//			{
		//				// we were lucky, the password we have is correct!
		//				FileModel.UpdateFrom(decryptForDisplayResult.Result); // do this before pushing the text to the editor
		//				_eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
		//				UpdateCanExecuteChanged();
		//				return; // exit from this goto-loop, we have a new decrypted file
		//			}
		//			if (decryptForDisplayResult.IsCanceled)
		//			{
		//				return; // we also exit from whole procedure (we could also move on to redisplay 
		//						// the password entry, as below, in case of error, which we interpret as wrong password).
		//			}
		//			StatusBarModel.ShowEncryptedBar(); // Error, i.e. wrong password, show EncryptedBar
		//		}

		//		// As we tested that it's valid VisualCrypt in the open routine, we can assume we are here because
		//		// the password was wrong. So we ask the user again..:
		//		bool result2 = await SetPasswordAsync(SetPasswordDialogMode.CorrectPassword);
		//		if (result2 == false)
		//			return; // The user prefers to look at the cipher!
		//					// We have another password, from the user, we try again!
		//		goto tryDecryptLoadFileWithCurrentPassword;
		//	}
		//	catch (Exception e)
		//	{
		//		_messageBoxService.ShowError(e);
		//	}
		//}

		//LongRunningOperation StartLongRunnungOperation(string description)
		//{
		//	StatusBarModel.ShowProgressBar(description);

		//	Action<EncryptionProgress> updateProgressBar = encryptionProgress =>
		//	{
		//		StatusBarModel.ProgressPercent = encryptionProgress.Percent;
		//		StatusBarModel.ProgressMessage = encryptionProgress.Message;
		//	};

		//	var switchBackToPreviousBar = FileModel.IsEncrypted
		//		? (Action)StatusBarModel.ShowEncryptedBar
		//		: StatusBarModel.ShowPlainTextBar;

		//	return new LongRunningOperation(updateProgressBar, switchBackToPreviousBar);
		//}
		//public void CancelLongRunningOperation()
		//{
		//	_longRunningOperation.Cancel();
		//}

		//void UpdateCanExecuteChanged()
		//{
		//	// TODO: many more Commands, like Replace must be updated
		//	//_exportCommand.RaiseCanExecuteChanged();
		//	_encryptEditorContentsCommand.RaiseCanExecuteChanged();
		//	_decryptEditorContentsCommand.RaiseCanExecuteChanged();
		//}



		//#endregion

		//#region SaveCommand

		//DelegateCommand _saveCommand;

		//public DelegateCommand SaveCommand
		//{
		//	get { return CreateCommand(ref _saveCommand, ExecuteSaveCommand, () => FileModel.SaveEncoding != null); }
		//}

		//void ExecuteSaveCommand()
		//{
		//	ExecuteSaveCommandsCommon(false);
		//}

		//async void ExecuteSaveCommandsCommon(bool isSaveAs)
		//{
		//	try
		//	{
		//		// This is the simple case, we can 'just save'.
		//		if (FileModel.IsEncrypted && !isSaveAs && CheckFilenameForQuickSave())
		//		{
		//			var response = _encryptionService.SaveEncryptedFile(FileModel);
		//			if (!response.IsSuccess)
		//				throw new Exception(response.Error);
		//			FileModel.IsDirty = false;
		//		}
		//		// This is the case where we need a new filename and can then also 'just save'.
		//		else if (FileModel.IsEncrypted && (isSaveAs || !CheckFilenameForQuickSave()))
		//		{
		//			string suggestedFilename = null;
		//			if (isSaveAs)
		//				suggestedFilename = FileModel.ShortFilename;

		//			var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
		//			if (pickFileResult.Item1)
		//			{
		//				FileModel.ShortFilename = pickFileResult.Item2;
		//				var response = _encryptionService.SaveEncryptedFile(FileModel);
		//				if (!response.IsSuccess)
		//					throw new Exception(response.Error);
		//				FileModel.IsDirty = false;
		//			}
		//		}
		//		// And in that case we need a different strategy: Encrypt and THEN save.
		//		else
		//		{
		//			if (FileModel.IsEncrypted)
		//				throw new InvalidOperationException("We assert confusion!");
		//			await EncryptAndThenSave(isSaveAs);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		_messageBoxService.ShowError(e);
		//	}
		//}

		//async Task EncryptAndThenSave(bool isSaveAs)
		//{
		//	// We have been called from Save/SaveAs because the file is not encrypted yet.
		//	// We are still in a try/catch block

		//	// To encrypt and then save, we must sure we have a password:
		//	if (!PasswordInfo.IsPasswordSet)
		//	{
		//		bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncryptAndSave);
		//		if (result == false)
		//			return;
		//	}
		//	// Then we must sure we have the file name:
		//	if (isSaveAs || !CheckFilenameForQuickSave())
		//	{
		//		string suggestedFilename = null;
		//		if (isSaveAs)
		//			suggestedFilename = FileModel.ShortFilename;

		//		var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
		//		if (!pickFileResult.Item1)
		//			return;
		//		FileModel.ShortFilename = pickFileResult.Item2;
		//	}
		//	// No we have password and filename, we can now encrypt and save in one step.
		//	// We will not replace FileManager.FileModel because we continue editing the same cleartext.
		//	string editorClearText = await EditorSendsTextAsync();

		//	using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationEncryptAndSave))
		//	{
		//		var encryptAndSaveFileResponse =
		//			await
		//				Task.Run(
		//					() =>
		//						_encryptionService.EncryptAndSaveFile(FileModel, editorClearText, new RoundsExponent(_settingsManager.CryptographySettings.LogRounds), _longRunningOperation.Context));

		//		if (encryptAndSaveFileResponse.IsSuccess)
		//		{
		//			string visualCryptTextSaved = encryptAndSaveFileResponse.Result;
		//			// We are done with successful saving. Show that we did encrypt the text:
		//			_eventAggregator.GetEvent<EditorReceivesText>().Publish(visualCryptTextSaved);
		//			StatusBarModel.ShowEncryptedBar();
		//			await Task.Delay(2000);
		//			StatusBarModel.ShowPlainTextBar();
		//			// Redisplay the clear text, to allow continue editing.
		//			FileModel.IsDirty = false;

		//			_eventAggregator.GetEvent<EditorReceivesText>().Publish(editorClearText);

		//			UpdateCanExecuteChanged();
		//			return;
		//		}
		//		if (encryptAndSaveFileResponse.IsCanceled)
		//		{
		//			return; // Cancel means the user can continue editing clear text
		//		}
		//		// other error, switch back to PlainTextBar
		//		StatusBarModel.ShowPlainTextBar();
		//		throw new Exception(encryptAndSaveFileResponse.Error);
		//	}
		//}



		//#endregion

		//#region SaveAsCommand

		//DelegateCommand _saveAsCommand;

		//public DelegateCommand SaveAsCommand
		//{
		//	get
		//	{
		//		return CreateCommand(ref _saveAsCommand, ExecuteSaveAsCommand, () => FileModel.SaveEncoding != null);
		//	}
		//}

		//void ExecuteSaveAsCommand()
		//{
		//	ExecuteSaveCommandsCommon(true);
		//}

		//#endregion

		//#region ExportCommand

		//DelegateCommand _exportCommand;

		//public DelegateCommand ExportCommand
		//{
		//	get
		//	{
		//		return CreateCommand(ref _exportCommand, ExecuteExportCommand,
		//			() => !FileModel.IsEncrypted && FileModel.SaveEncoding != null);
		//	}
		//}

		//async void ExecuteExportCommand()
		//{
		//	try
		//	{
		//		var editorClearText = await EditorSendsTextAsync();

		//		if (editorClearText == null)
		//			throw new InvalidOperationException("The text received from the editor was null.");

		//		string title = string.Format(CultureInfo.InvariantCulture, "Export Clear Text (Encoding: {0})",
		//			FileModel.SaveEncodingName);

		//		string suggestedFilename = FileModel.ShortFilename.ReplaceCaseInsensitive(PortableConstants.DotVisualCrypt,
		//			string.Empty);
		//		var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.Text, DialogDirection.Save, title);
		//		if (pickFileResult.Item1)
		//		{
		//			byte[] encodedTextBytes = FileModel.SaveEncoding.GetBytes(editorClearText);
		//			_fileService.WriteAllBytes(pickFileResult.Item2, encodedTextBytes);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		_messageBoxService.ShowError(e);
		//	}
		//}

		//static async Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter,
		//	DialogDirection dialogDirection, string title = null)
		//{
		//	throw new NotImplementedException();
		//	//FileDialog fileDialog;
		//	//if (dialogDirection == DialogDirection.Open)
		//	//    fileDialog = new OpenFileDialog();
		//	//else
		//	//    fileDialog = new SaveFileDialog();

		//	//if (title != null)
		//	//    fileDialog.Title = title;

		//	//if (!string.IsNullOrEmpty(suggestedFilename))
		//	//    fileDialog.FileName = suggestedFilename;

		//	//fileDialog.InitialDirectory = SettingsManager.CurrentDirectoryName;
		//	//if (diaglogFilter == DialogFilter.VisualCrypt)
		//	//{
		//	//    fileDialog.DefaultExt = Constants.VisualCryptDialogFilter_DefaultExt;
		//	//    fileDialog.Filter = Constants.VisualCryptDialogFilter;
		//	//}
		//	//else
		//	//{
		//	//    fileDialog.DefaultExt = Constants.TextDialogFilter_DefaultExt;
		//	//    fileDialog.Filter = Constants.TextDialogFilter;
		//	//}

		//	//var tcs = new TaskCompletionSource<Tuple<bool, string>>();

		//	//var okClicked = fileDialog.ShowDialog() == true;
		//	//var selectedFilename = fileDialog.FileName;

		//	//tcs.SetResult(new Tuple<bool, string>(okClicked, selectedFilename));
		//	//return await tcs.Task;
		//}

		//#endregion

		//#region EncryptEditorContentsCommand

		//DelegateCommand _encryptEditorContentsCommand;

		//public DelegateCommand EncryptEditorContentsCommand
		//{
		//	get
		//	{
		//		return CreateCommand(ref _encryptEditorContentsCommand, ExecuteEncryptEditorContentsCommand,
		//			() => !FileModel.IsEncrypted && FileModel.SaveEncoding != null);
		//	}
		//}


		//async void ExecuteEncryptEditorContentsCommand()
		//{
		//	try
		//	{
		//		if (!PasswordInfo.IsPasswordSet)
		//		{
		//			bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncrypt);
		//			if (result == false)
		//				return;
		//		}

		//		string textBufferContents = await EditorSendsTextAsync();

		//		using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationEncryption))
		//		{
		//			var createEncryptedFileResponse =
		//				await
		//					Task.Run(
		//						() =>
		//							_encryptionService.EncryptForDisplay(FileModel, textBufferContents, new RoundsExponent(_settingsManager.CryptographySettings.LogRounds), _longRunningOperation.Context));
		//			if (createEncryptedFileResponse.IsSuccess)
		//			{
		//				FileModel.UpdateFrom(createEncryptedFileResponse.Result);  // do this BEFORE pushing the text to the editor
		//				_eventAggregator.GetEvent<EditorReceivesText>().Publish(createEncryptedFileResponse.Result.VisualCryptText);
		//				UpdateCanExecuteChanged();
		//				return;
		//			}
		//			if (createEncryptedFileResponse.IsCanceled)
		//				return;
		//			// other error, switch back to PlainTextBar and show error
		//			StatusBarModel.ShowPlainTextBar();
		//			_messageBoxService.ShowError(createEncryptedFileResponse.Error);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		_messageBoxService.ShowError(e);
		//	}
		//}

		//#endregion

		//#region DecryptEditorContentsCommand

		//DelegateCommand _decryptEditorContentsCommand;


		//public DelegateCommand DecryptEditorContentsCommand
		//{
		//	get
		//	{
		//		return CreateCommand(ref _decryptEditorContentsCommand, ExecuteDecryptEditorContentsCommand,
		//			() => FileModel.SaveEncoding != null);
		//	}
		//}



		//async void ExecuteDecryptEditorContentsCommand()
		//{
		//	try
		//	{
		//		if (!PasswordInfo.IsPasswordSet)
		//		{
		//			bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecrypt);
		//			if (result == false)
		//				return;
		//		}

		//		string textBufferContents = await EditorSendsTextAsync();

		//		using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationDecryption))
		//		{
		//			var decryptForDisplayResult =
		//				await
		//					Task.Run(
		//						() =>
		//							_encryptionService.DecryptForDisplay(FileModel, textBufferContents, _longRunningOperation.Context));
		//			if (decryptForDisplayResult.IsSuccess)
		//			{
		//				FileModel.UpdateFrom(decryptForDisplayResult.Result); // do this BEFORE pushing the text to the editor
		//				_eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
		//				UpdateCanExecuteChanged();
		//				return;
		//			}
		//			if (decryptForDisplayResult.IsCanceled)
		//				return;
		//			// other error, switch back to EncryptedBar
		//			StatusBarModel.ShowEncryptedBar();
		//			_messageBoxService.ShowError(decryptForDisplayResult.Error);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		_messageBoxService.ShowError(e);
		//	}
		//}



		//#endregion

		//#region private methods

		//bool ConfirmToDiscardText()
		//{
		//	return true;
		//	//if (FileModel.IsDirty)
		//	//    return
		//	//        (_messageBoxService.Show("Discard changes?", Constants.ProductName, RequestButton.OKCancel,
		//	//            RequestImage.Question) ==
		//	//         RequestResult.OK);

		//	//return true;
		//}

		//#endregion




	}
}
