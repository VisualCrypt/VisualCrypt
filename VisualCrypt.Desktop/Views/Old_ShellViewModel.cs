﻿//using System;
//using System.ComponentModel;
//using System.ComponentModel.Composition;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Threading.Tasks;
//using System.Windows;
//using Microsoft.Practices.Prism.Logging;
//using Microsoft.Practices.Prism.PubSubEvents;
//using Microsoft.Win32;
//using VisualCrypt.Cryptography.Portable;
//using VisualCrypt.Cryptography.Portable.Events;
//using VisualCrypt.Cryptography.Portable.Models;
//using VisualCrypt.Cryptography.Portable.MVVM;
//using VisualCrypt.Cryptography.Portable.Settings;
//using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
//using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
//using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;
//using VisualCrypt.Desktop.Services;
//using VisualCrypt.Desktop.Shared.App;
//using VisualCrypt.Desktop.Shared.Files;
//using VisualCrypt.Desktop.Shared.PrismSupport;
//using VisualCrypt.Language;

//namespace VisualCrypt.Desktop.Views
//{
//	[Export]
//	public class ShellViewModel : ViewModelBase
//	{
//		public readonly PasswordInfo PasswordInfo = new PasswordInfo();
//		public readonly StatusBarModel StatusBarModel = new StatusBarModel();
//		public readonly FileModel FileModel;

//		readonly IEventAggregator _eventAggregator;
//		readonly ILoggerFacade _logger;

//		readonly IMessageBoxService _messageBoxService;
//		readonly IEncryptionService _encryptionService;

//		readonly INavigationService _navigationService;
//		readonly IPasswordDialogDispatcher _passwordDialogDispatcher;


//		readonly ISettingsManager _settingsManager;
//		readonly IFileService _fileService;



//		LongRunningOperation _longRunningOperation;

//		[ImportingConstructor]
//		public ShellViewModel(IEventAggregator eventAggregator, ILoggerFacade logger,
//			IMessageBoxService messageBoxService, IEncryptionService encryptionService,
//			INavigationService navigationService, IPasswordDialogDispatcher passwordDialogDispatcher,
//			   ISettingsManager settingsManager, IFileService fileService)
//		{
//			_eventAggregator = eventAggregator;
//			_logger = logger;

//			_messageBoxService = messageBoxService;
//			_encryptionService = encryptionService;

//			_navigationService = navigationService;
//			_passwordDialogDispatcher = passwordDialogDispatcher;


//			_settingsManager = settingsManager;
//			_fileService = fileService;

//			_eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Subscribe(OnEditorSendsStatusBarInfo);
//			_eventAggregator.GetEvent<FileModelChanged>().Subscribe(ExecuteEditorSendsTextCallback);
//		}


//		public void OpenFromCommandLineOrNew(string[] args)
//		{
           
//			if (args.Length == 2 && !string.IsNullOrWhiteSpace(args[1]))
//			{
//				var fileName = args[1];
//				_logger.Log(string.Format(CultureInfo.InvariantCulture, "Loading file from Commandline: {0}", fileName),
//					Category.Info,
//					Priority.None);
//				OpenFileCommon(fileName);
//			}
//			else
//			{
//				ExecuteNewCommand();
//				_logger.Log("Started with new file - Ready.", Category.Info, Priority.None);
//			}
//		}

//		void ExecuteEditorSendsTextCallback(FileModelChanged args)
//		{
//			if (args != null && args.Callback != null)
//				args.Callback(args.Text);
//		}

//		async Task<string> EditorSendsTextAsync()
//		{
//			var tcs = new TaskCompletionSource<string>();
//			_eventAggregator.GetEvent<EditorShouldSendText>()
//				.Publish(tcs.SetResult);
//			return await tcs.Task;
//		}

//		void OnEditorSendsStatusBarInfo(string statusBarInfo)
//		{
//			StatusBarText = statusBarInfo;
//		}

//		#region INPC Properties

//		public string StatusBarText
//		{
//			get { return _statusBarText; }
//			set
//			{
//				if (_statusBarText == value) return;
//				_statusBarText = value;
//				OnPropertyChanged(() => StatusBarText);
//			}
//		}

//		string _statusBarText;

//		#endregion

//		#region ExitCommand

//		public DelegateCommand<CancelEventArgs> ExitCommand
//		{
//			get { return CreateCommand(ref _exitCommand, ExecuteExitCommand, e => true); }
//		}

//		DelegateCommand<CancelEventArgs> _exitCommand;

//		bool _isExitConfirmed;

//		void ExecuteExitCommand(CancelEventArgs e)
//		{
//			bool isInvokedFromWindowCloseEvent = e != null;

//			if (isInvokedFromWindowCloseEvent)
//			{
//				if (_isExitConfirmed)
//					return;
//				if (ConfirmToDiscardText())
//					return;
//				e.Cancel = true;
//			}
//			else
//			{
//				if (ConfirmToDiscardText())
//				{
//					_isExitConfirmed = true;
//					Application.Current.Shutdown();
//				}
//			}
//		}

//		#endregion

//		#region NewCommand

//		DelegateCommand _newCommand;

//		public DelegateCommand NewCommand
//		{
//			get { return CreateCommand(ref _newCommand, ExecuteNewCommand, () => true); }
//		}

//		void ExecuteNewCommand()
//		{
//			if (!ConfirmToDiscardText())
//				return;
//			FileService.FileModel = FileModel.EmptyCleartext();
//			_eventAggregator.GetEvent<EditorReceivesText>().Publish(FileService.FileModel.ClearTextContents);
//		}

//		#endregion

//		#region ImportWithEncodingCommand

//		DelegateCommand _importWithEncodingCommand;

//		public DelegateCommand ImportWithEncodingCommand
//		{
//			get { return CreateCommand(ref _importWithEncodingCommand, ExecuteImportWithEncodingCommand, () => true); }
//		}

//		async void ExecuteImportWithEncodingCommand()
//		{
//			try
//			{
//				if (!ConfirmToDiscardText())
//					return;

//				var importEncoding = await WindowManager.GetDialogFromShowDialogAsyncWhenClosed<ImportEncodingDialog>();


//				if (importEncoding.DialogResult == true)
//				{
//					if (!ConfirmToDiscardText())
//						return;

//					var selectedEncoding = importEncoding.SelectedEncodingInfo.GetEncoding();

//					string title = string.Format(CultureInfo.InvariantCulture, "Import With Encoding: {0})",
//						selectedEncoding);

//					var pickFileResult = await PickFileAsync(null, DialogFilter.Text, DialogDirection.Open, title);
//					if (pickFileResult.Item1)
//					{
//						string filename = pickFileResult.Item2;
//						var shortFilename = Path.GetFileName(filename);
//						string importedString = File.ReadAllText(filename, selectedEncoding);
//						FileService.FileModel = FileModel.Cleartext(filename, shortFilename, importedString,
//							selectedEncoding);
//						_eventAggregator.GetEvent<EditorReceivesText>().Publish(FileService.FileModel.ClearTextContents);
//					}
//				}
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		#endregion

//		#region HelpCommand

//		DelegateCommand _helpCommand;

//		public DelegateCommand HelpCommand
//		{
//			get { return CreateCommand(ref _helpCommand, ExecuteHelpCommand, () => true); }
//		}

//		void ExecuteHelpCommand()
//		{
//			try
//			{
//				using (
//					var process = new Process {StartInfo = {UseShellExecute = true, FileName = Loc.Strings.uriHelpUrl}})
//					process.Start();
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		#endregion

//		#region AboutCommand

//		public DelegateCommand AboutCommand
//		{
//			get { return CreateCommand(ref _aboutCommand, ExecuteAboutCommand, () => true); }
//		}

//		DelegateCommand _aboutCommand;

//		async void ExecuteAboutCommand()
//		{
//			try
//			{
//				await WindowManager.GetDialogFromShowDialogAsyncWhenClosed<AboutDialog>();
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		#endregion

//		#region LogCommand

//		public DelegateCommand LogCommand
//		{
//			get { return CreateCommand(ref _logCommand, ExecuteLogCommand, () => true); }
//		}


//		DelegateCommand _logCommand;


//		async void ExecuteLogCommand()
//		{
//			try
//			{
//				await WindowManager.ShowWindowAsyncAndWaitForClose<LogWindow>();
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		#endregion

//		#region OpenCommand

//		DelegateCommand _openCommand;

//		public DelegateCommand OpenCommand
//		{
//			get { return CreateCommand(ref _openCommand, ExecuteOpenCommand, () => true); }
//		}

//		async void ExecuteOpenCommand()
//		{
//			if (!ConfirmToDiscardText())
//				return;

//			var pickFileResult = await PickFileAsync(null, DialogFilter.VisualCrypt, DialogDirection.Open);
//			if (pickFileResult.Item1)
//			{
//				OpenFileCommon(pickFileResult.Item2);
//			}
//		}


//		async void OpenFileCommon(string filename)
//		{
//			if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
//				return;

//			try
//			{
//				var openFileResponse = _encryptionService.OpenFile(filename);
//				if (!openFileResponse.IsSuccess)
//				{
//					_messageBoxService.ShowError(openFileResponse.Error);
//					return;
//				}
//				if (openFileResponse.Result.SaveEncoding == null)
//				{
//					if (_messageBoxService.Show(
//						"This file is neither text nor VisualCrypt - display with Hex View?\r\n\r\n" +
//						"If file is very large the editorView may become less responsive.", "Binary File",
//						RequestButton.OKCancel, RequestImage.Warning) == RequestResult.Cancel)
//						return;
//				}
//				FileService.FileModel = openFileResponse.Result;
//				SettingsManager.CurrentDirectoryName = Path.GetDirectoryName(filename);

//				var ert = _eventAggregator.GetEvent<EditorReceivesText>();
//				var text = FileService.FileModel.IsEncrypted
//					? FileService.FileModel.VisualCryptText
//					: FileService.FileModel.ClearTextContents;
//				ert.Publish(text);


//				// if the loaded file was cleartext we are all done
//				if (!FileService.FileModel.IsEncrypted)
//					return;

//				// if it's encrypted, check if we have SOME password
//				if (!PasswordManager.PasswordInfo.IsPasswordSet)
//				{
//					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecryptLoadedFile);
//					if (result == false)
//						return; // The user prefers to look at the cipher!
//				}

//				tryDecryptLoadFileWithCurrentPassword:

//				// We have a password, but we don't know if it's the right one. We must try!
//				using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationDecryptOpenedFile))
//				{
//					var decryptForDisplayResult =
//						await Task.Run(() => _encryptionService.DecryptForDisplay(FileService.FileModel,
//							FileService.FileModel.VisualCryptText, _longRunningOperation.Context));
//					if (decryptForDisplayResult.IsSuccess)
//					{
//						// we were lucky, the password we have is correct!
//						FileService.FileModel = decryptForDisplayResult.Result;
//							// do this before pushing the text to the editorView
//						_eventAggregator.GetEvent<EditorReceivesText>()
//							.Publish(decryptForDisplayResult.Result.ClearTextContents);
//						UpdateCanExecuteChanged();
//						return; // exit from this goto-loop, we have a new decrypted file
//					}
//					if (decryptForDisplayResult.IsCanceled)
//					{
//						return; // we also exit from whole procedure (we could also move on to redisplay 
//						// the password entry, as below, in case of error, which we interpret as wrong password).
//					}
//					FileService.ShowEncryptedBar(); // Error, i.e. wrong password, show EncryptedBar
//				}

//				// As we tested that it's valid VisualCrypt in the open routine, we can assume we are here because
//				// the password was wrong. So we ask the user again..:
//				bool result2 = await SetPasswordAsync(SetPasswordDialogMode.CorrectPassword);
//				if (result2 == false)
//					return; // The user prefers to look at the cipher!
//				// We have another password, from the user, we try again!
//				goto tryDecryptLoadFileWithCurrentPassword;
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		public void OpenFileFromDragDrop(string dropFilename)
//		{
//			if (!ConfirmToDiscardText())
//				return;
//			OpenFileCommon(dropFilename);
//		}

//		#endregion

//		#region EncryptEditorContentsCommand

//		DelegateCommand _encryptEditorContentsCommand;

//		public DelegateCommand EncryptEditorContentsCommand
//		{
//			get
//			{
//				return CreateCommand(ref _encryptEditorContentsCommand, ExecuteEncryptEditorContentsCommand,
//					() => !FileService.FileModel.IsEncrypted && FileService.FileModel.SaveEncoding != null);
//			}
//		}


//		async void ExecuteEncryptEditorContentsCommand()
//		{
//			try
//			{
//				if (!PasswordManager.PasswordInfo.IsPasswordSet)
//				{
//					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncrypt);
//					if (result == false)
//						return;
//				}

//				string textBufferContents = await EditorSendsTextAsync();

//				using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationEncryption))
//				{
//					var createEncryptedFileResponse =
//						await
//							Task.Run(
//								() =>
//									_encryptionService.EncryptForDisplay(FileService.FileModel, textBufferContents,
//										new RoundsExponent(SettingsManager.CryptographySettings.LogRounds),
//										_longRunningOperation.Context));
//					if (createEncryptedFileResponse.IsSuccess)
//					{
//						FileService.FileModel = createEncryptedFileResponse.Result;
//							// do this before pushing the text to the editorView
//						_eventAggregator.GetEvent<EditorReceivesText>()
//							.Publish(createEncryptedFileResponse.Result.VisualCryptText);
//						UpdateCanExecuteChanged();
//						return;
//					}
//					if (createEncryptedFileResponse.IsCanceled)
//						return;
//					// other error, switch back to PlainTextBar and show error
//					FileService.ShowPlainTextBar();
//					_messageBoxService.ShowError(createEncryptedFileResponse.Error);
//				}
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		#endregion

//		#region DecryptEditorContentsCommand

//		DelegateCommand _decryptEditorContentsCommand;

//		public DelegateCommand DecryptEditorContentsCommand
//		{
//			get
//			{
//				return CreateCommand(ref _decryptEditorContentsCommand, ExecuteDecryptEditorContentsCommand,
//					() => FileService.FileModel.SaveEncoding != null);
//			}
//		}


//		async void ExecuteDecryptEditorContentsCommand()
//		{
//			try
//			{
//				if (!PasswordManager.PasswordInfo.IsPasswordSet)
//				{
//					bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndDecrypt);
//					if (result == false)
//						return;
//				}

//				string textBufferContents = await EditorSendsTextAsync();

//				using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationDecryption))
//				{
//					var decryptForDisplayResult =
//						await
//							Task.Run(
//								() =>
//									_encryptionService.DecryptForDisplay(FileService.FileModel, textBufferContents,
//										_longRunningOperation.Context));
//					if (decryptForDisplayResult.IsSuccess)
//					{
//						FileService.FileModel = decryptForDisplayResult.Result;
//							// do this before pushing the text to the editorView
//						_eventAggregator.GetEvent<EditorReceivesText>()
//							.Publish(decryptForDisplayResult.Result.ClearTextContents);
//						UpdateCanExecuteChanged();
//						return;
//					}
//					if (decryptForDisplayResult.IsCanceled)
//						return;
//					// other error, switch back to EncryptedBar
//					FileService.ShowEncryptedBar();
//					_messageBoxService.ShowError(decryptForDisplayResult.Error);
//				}
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		LongRunningOperation StartLongRunnungOperation(string description)
//		{
//			FileService.ShowWorkingBar(description);

//			Action<EncryptionProgress> updateProgressBar = encryptionProgress =>
//			{
//				FileService.BindableFileInfo.ProgressPercent = encryptionProgress.Percent;
//				FileService.BindableFileInfo.ProgressMessage = encryptionProgress.Message;
//			};

//			var switchBackToPreviousBar = FileService.FileModel.IsEncrypted
//				? (Action) FileService.ShowEncryptedBar
//				: FileService.ShowPlainTextBar;

//			return new LongRunningOperation(updateProgressBar, switchBackToPreviousBar);
//		}

//		#endregion

//		#region SaveCommand

//		DelegateCommand _saveCommand;

//		public DelegateCommand SaveCommand
//		{
//			get
//			{
//				return CreateCommand(ref _saveCommand, ExecuteSaveCommand,
//					() => FileService.FileModel.SaveEncoding != null);
//			}
//		}

//		void ExecuteSaveCommand()
//		{
//			ExecuteSaveCommandsCommon(false);
//		}

//		async void ExecuteSaveCommandsCommon(bool isSaveAs)
//		{
//			try
//			{
//				// This is the simple case, we can 'just save'.
//				if (FileService.FileModel.IsEncrypted && !isSaveAs && FileService.CheckFilenameForQuickSave())
//				{
//					var response = _encryptionService.SaveEncryptedFile(FileService.FileModel);
//					if (!response.IsSuccess)
//						throw new Exception(response.Error);
//					FileService.FileModel.IsDirty = false;
//				}
//				// This is the case where we need a new filename and can then also 'just save'.
//				else if (FileService.FileModel.IsEncrypted && (isSaveAs || !FileService.CheckFilenameForQuickSave()))
//				{
//					string suggestedFilename = null;
//					if (isSaveAs)
//						suggestedFilename = FileService.FileModel.Filename;

//					var pickFileResult =
//						await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
//					if (pickFileResult.Item1)
//					{
//						FileService.FileModel.Filename = pickFileResult.Item2;
//						var response = _encryptionService.SaveEncryptedFile(FileService.FileModel);
//						if (!response.IsSuccess)
//							throw new Exception(response.Error);
//						FileService.FileModel.IsDirty = false;
//					}
//				}
//				// And in that case we need a different strategy: Encrypt and THEN save.
//				else
//				{
//					if (FileService.FileModel.IsEncrypted)
//						throw new InvalidOperationException("We assert confusion!");
//					await EncryptAndThenSave(isSaveAs);
//				}
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		async Task EncryptAndThenSave(bool isSaveAs)
//		{
//			// We have been called from Save/SaveAs because the file is not encrypted yet.
//			// We are still in a try/catch block

//			// To encrypt and then save, we must sure we have a password:
//			if (!PasswordManager.PasswordInfo.IsPasswordSet)
//			{
//				bool result = await SetPasswordAsync(SetPasswordDialogMode.SetAndEncryptAndSave);
//				if (result == false)
//					return;
//			}
//			// Then we must sure we have the file name:
//			if (isSaveAs || !FileService.CheckFilenameForQuickSave())
//			{
//				string suggestedFilename = null;
//				if (isSaveAs)
//					suggestedFilename = FileService.FileModel.Filename;

//				var pickFileResult =
//					await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
//				if (!pickFileResult.Item1)
//					return;
//				FileService.FileModel.Filename = pickFileResult.Item2;
//			}
//			// No we have password and filename, we can now encrypt and save in one step.
//			// We will not replace FileService.FileModel because we continue editing the same cleartext.
//			string editorClearText = await EditorSendsTextAsync();

//			using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationEncryptAndSave))
//			{
//				var encryptAndSaveFileResponse =
//					await
//						Task.Run(
//							() =>
//								_encryptionService.EncryptAndSaveFile(FileService.FileModel, editorClearText,
//									new RoundsExponent(SettingsManager.CryptographySettings.LogRounds),
//									_longRunningOperation.Context));

//				if (encryptAndSaveFileResponse.IsSuccess)
//				{
//					string visualCryptTextSaved = encryptAndSaveFileResponse.Result;
//					// We are done with successful saving. Show that we did encrypt the text:
//					_eventAggregator.GetEvent<EditorReceivesText>().Publish(visualCryptTextSaved);
//					FileService.ShowEncryptedBar();
//					await Task.Delay(2000);
//					FileService.ShowPlainTextBar();
//					// Redisplay the clear text, to allow continue editing.
//					FileService.FileModel.IsDirty = false;

//					_eventAggregator.GetEvent<EditorReceivesText>().Publish(editorClearText);

//					UpdateCanExecuteChanged();
//					return;
//				}
//				if (encryptAndSaveFileResponse.IsCanceled)
//				{
//					return; // Cancel means the user can continue editing clear text
//				}
//				// other error, switch back to PlainTextBar
//				FileService.ShowPlainTextBar();
//				throw new Exception(encryptAndSaveFileResponse.Error);
//			}
//		}

//		#endregion

//		#region SaveAsCommand

//		DelegateCommand _saveAsCommand;

//		public DelegateCommand SaveAsCommand
//		{
//			get
//			{
//				return CreateCommand(ref _saveAsCommand, ExecuteSaveAsCommand,
//					() => FileService.FileModel.SaveEncoding != null);
//			}
//		}

//		void ExecuteSaveAsCommand()
//		{
//			ExecuteSaveCommandsCommon(true);
//		}

//		#endregion

//		#region ExportCommand

//		DelegateCommand _exportCommand;

//		public DelegateCommand ExportCommand
//		{
//			get
//			{
//				return CreateCommand(ref _exportCommand, ExecuteExportCommand,
//					() => !FileService.FileModel.IsEncrypted && FileService.FileModel.SaveEncoding != null);
//			}
//		}

//		async void ExecuteExportCommand()
//		{
//			try
//			{
//				var editorClearText = await EditorSendsTextAsync();

//				if (editorClearText == null)
//					throw new InvalidOperationException("The text received from the editorView was null.");

//				string title = string.Format(CultureInfo.InvariantCulture, "Export Clear Text (Encoding: {0})",
//					FileService.FileModel.SaveEncoding.EncodingName);

//				string suggestedFilename =
//					FileService.FileModel.Filename.ReplaceCaseInsensitive(PortableConstants.DotVisualCrypt,
//						string.Empty);
//				var pickFileResult =
//					await PickFileAsync(suggestedFilename, DialogFilter.Text, DialogDirection.Save, title);
//				if (pickFileResult.Item1)
//				{
//					byte[] encodedTextBytes = FileService.FileModel.SaveEncoding.GetBytes(editorClearText);
//					File.WriteAllBytes(pickFileResult.Item2, encodedTextBytes);
//				}
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		#endregion

//		#region ShowSetPasswordDialogCommand

//		DelegateCommand _showSetPasswordDialogCommand;

//		public DelegateCommand ShowSetPasswordDialogCommand
//		{
//			get
//			{
//				if (_showSetPasswordDialogCommand == null)
//					_showSetPasswordDialogCommand = DelegateCommand.FromAsyncHandler(
//						ExecuteShowSetPasswordDialogCommand, () => true);
//				return _showSetPasswordDialogCommand;
//			}
//		}

//		async Task ExecuteShowSetPasswordDialogCommand()
//		{
//			try
//			{
//				if (PasswordManager.PasswordInfo.IsPasswordSet)
//					await SetPasswordAsync(SetPasswordDialogMode.Change);
//				else
//				{
//					await SetPasswordAsync(SetPasswordDialogMode.Set);
//				}
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}


//		/// <summary>
//		/// Returns true, if the user did positively set a password, otherwise always false;
//		/// </summary>
//		async Task<bool> SetPasswordAsync(SetPasswordDialogMode setPasswordDialogMode)
//		{
//			ParamsProvider.SetParams(typeof (SetPasswordDialog), setPasswordDialogMode);
//			return await WindowManager.GetBoolFromShowDialogAsyncWhenClosed<SetPasswordDialog>();
//		}

//		#endregion

//		#region ShowSettingsDialogCommand

//		DelegateCommand _showSettingsDialogCommand;

//		public DelegateCommand ShowSettingsDialogCommand
//		{
//			get
//			{
//				if (_showSettingsDialogCommand == null)
//					_showSettingsDialogCommand = DelegateCommand.FromAsyncHandler(ExecuteShowSettingsDialogCommand,
//						() => true);
//				return _showSettingsDialogCommand;
//			}
//		}

//		async Task ExecuteShowSettingsDialogCommand()
//		{
//			try
//			{
//				await WindowManager.GetBoolFromShowDialogAsyncWhenClosed<SettingsDialog>();
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e);
//			}
//		}

//		#endregion

//		#region DefaultsCommand

//		public DelegateCommand ClearPasswordCommand
//		{
//			get { return CreateCommand(ref _clearPasswordCommand, ExecuteClearPasswordCommand, () => true); }
//		}

//		DelegateCommand _clearPasswordCommand;

//		void ExecuteClearPasswordCommand()
//		{
//			try
//			{
//				var clearPasswordResponse = _encryptionService.ClearPassword();
//				if (!clearPasswordResponse.IsSuccess)
//				{
//					_messageBoxService.ShowError(clearPasswordResponse.Error);
//					return;
//				}
//				PasswordManager.PasswordInfo.IsPasswordSet = false;
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e.Message);
//			}
//		}

//		#endregion

//		#region CopyAllCommand

//		public DelegateCommand CopyAllCommand
//		{
//			get { return CreateCommand(ref _copyAllCommand, ExecuteCopyAllCommand, () => true); }
//		}

//		DelegateCommand _copyAllCommand;

//		void ExecuteCopyAllCommand()
//		{
//			try
//			{
//				var cipherText = FileModel.VisualCryptText;
//				Clipboard.SetText(cipherText, TextDataFormat.Text);
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e.Message);
//			}
//		}

//		#endregion

//		#region SwitchLanguageCommand

//		public DelegateCommand<string> SwitchLanguageCommand
//		{
//			get { return CreateCommand(ref _switchLanguageCommand, ExecuteSwitchLanguageCommand, s => true); }
//		}

//		DelegateCommand<string> _switchLanguageCommand;

//		void ExecuteSwitchLanguageCommand(string loc)
//		{
//			try
//			{
//				Loc.SetLanguage(loc);
//			}
//			catch (Exception e)
//			{
//				_messageBoxService.ShowError(e.Message);
//			}
//		}

//		#endregion

//		#region private methods

//		void UpdateCanExecuteChanged()
//		{
//			// TODO: many more Commands, like Replace must be updated
//			_exportCommand.RaiseCanExecuteChanged();
//			_encryptEditorContentsCommand.RaiseCanExecuteChanged();
//			_decryptEditorContentsCommand.RaiseCanExecuteChanged();
//		}

//		bool ConfirmToDiscardText()
//		{
//			if (FileService.FileModel.IsDirty)
//				return
//					(_messageBoxService.Show("Discard changes?", Constants.ProductName, RequestButton.OKCancel,
//						RequestImage.Question) ==
//					 RequestResult.OK);

//			return true;
//		}

//		static async Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter,
//			DialogDirection dialogDirection, string title = null)
//		{
//			FileDialog fileDialog;
//			if (dialogDirection == DialogDirection.Open)
//				fileDialog = new OpenFileDialog();
//			else
//				fileDialog = new SaveFileDialog();

//			if (title != null)
//				fileDialog.Title = title;

//			if (!string.IsNullOrEmpty(suggestedFilename))
//				fileDialog.FileName = suggestedFilename;

//			fileDialog.InitialDirectory = SettingsManager.CurrentDirectoryName;
//			if (diaglogFilter == DialogFilter.VisualCrypt)
//			{
//				fileDialog.DefaultExt = Constants.VisualCryptDialogFilter_DefaultExt;
//				fileDialog.Filter = Constants.VisualCryptDialogFilter;
//			}
//			else
//			{
//				fileDialog.DefaultExt = Constants.TextDialogFilter_DefaultExt;
//				fileDialog.Filter = Constants.TextDialogFilter;
//			}

//			var tcs = new TaskCompletionSource<Tuple<bool, string>>();

//			var okClicked = fileDialog.ShowDialog() == true;
//			var selectedFilename = fileDialog.FileName;

//			tcs.SetResult(new Tuple<bool, string>(okClicked, selectedFilename));
//			return await tcs.Task;
//		}

//		#endregion

//		public void CancelLongRunningOperation()
//		{
//			_longRunningOperation.Cancel();
//		}
//	}
//}