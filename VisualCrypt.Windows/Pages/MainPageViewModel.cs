using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Cryptography.Portable.MVVM;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;
using VisualCrypt.Language;
using VisualCrypt.Windows.Controls;
using VisualCrypt.Windows.Controls.EditorSupport;
using VisualCrypt.Windows.Events;
using VisualCrypt.Windows.Models;
using VisualCrypt.Windows.Services;
using VisualCrypt.Windows.Static;
using VisualCrypt.Windows.V2;

namespace VisualCrypt.Windows.Pages
{
    sealed class MainPageViewModel : ViewModelBase, IActiveCleanup, IEditorContext
    {
        public readonly PasswordInfo PasswordInfo = new PasswordInfo();
        public readonly StatusBarModel StatusBarModel = new StatusBarModel();
        public readonly FileModel FileModel;
       

        IFileModel IEditorContext.FileModel => FileModel;

        readonly IEncryptionService _encryptionService = new EncryptionService();
        readonly IMessageBoxService _messageBoxService = SharedInstances.MessageBoxService;
        readonly IEventAggregator _eventAggregator = SharedInstances.EventAggregator;
        IFrameNavigation _frameNavigation;
        LongRunningOperation _longRunningOperation;

        public MainPageViewModel(IFrameNavigation frameNavigation)
        {
            _frameNavigation = frameNavigation;
            _eventAggregator.GetEvent<EditorSendsText>().Subscribe(ExecuteEditorSendsTextCallback);
            _eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Subscribe(StatusBarModel.UpdateStatusBarText);
            FileModel = FileModel.EmptyCleartext();
        }
        

        public async Task OnNavigatedToCompletedAndLoaded(NavigationEventArgs e)
        {
            Debug.Assert(e?.Parameter is FilesPageCommandArgs);

            var command = (FilesPageCommandArgs)e.Parameter;
            switch (command.FilesPageCommand)
            {
                case FilesPageCommand.New:
                    ExecuteNewCommand();
                    break;
                case FilesPageCommand.Open:
                    await ExecuteOpenCommand(command.FileReference);
                    break;
                default:
                    throw new InvalidOperationException($"Unknwon command {command}");
            }
        }


       

        static void ExecuteEditorSendsTextCallback(EditorSendsText args)
        {
            Debug.Assert(args?.Callback != null);
            args.Callback(args.Text);
        }

        Task<string> EditorSendsTextAsync()
        {

            var tcs = new TaskCompletionSource<string>();
            _eventAggregator.GetEvent<EditorShouldSendText>()
                .Publish(delegate (string s)
                {
                    tcs.SetResult(s);
                });
            return tcs.Task;
        }






        #region NewCommand

        void ExecuteNewCommand()
        {
            if (!ConfirmToDiscardText())
                return;

            FileModel.UpdateFrom(FileModel.EmptyCleartext());

            var ert = _eventAggregator.GetEvent<EditorReceivesText>();
            ert.Publish(FileModel.ClearTextContents);
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

            _frameNavigation.Frame.Navigate(typeof(FilesPage));
            Cleanup();
        }

        #endregion

        #region PasswordCommand

        public DelegateCommand PasswordCommand
        {
            get
            {
                return CreateCommand(ref _passwordCommand, async () => { await ExecutePasswordCommand(); }, () => true);
            }
        }
        DelegateCommand _passwordCommand;


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
            _passwordPopup = new Popup();
            _passwordPopup.Child = new PasswordDialog(_encryptionService, setPasswordDialogMode, 
                ClosePasswordPopop,SetIsPasswordSet, PasswordInfo.IsPasswordSet);
            _passwordPopup.IsOpen = true;

            _popupTaskCompletionSource = new TaskCompletionSource<bool>();
            return await _popupTaskCompletionSource.Task;
        }
        Popup _passwordPopup;
        TaskCompletionSource<bool> _popupTaskCompletionSource;
        void ClosePasswordPopop(bool setClicked)
        {
            _popupTaskCompletionSource.SetResult(setClicked);
            _passwordPopup.IsOpen = false;
        }

        void SetIsPasswordSet(bool isPasswordSet)
        {
            PasswordInfo.IsPasswordSet = isPasswordSet;
        }

        #endregion

        #region OpenCommand

        async Task ExecuteOpenCommand(FileReference fileReference)
        {
            await OpenFileCommon(fileReference.DirectoryName);
        }

        async Task OpenFileCommon(string filename)
        {
            Debug.Assert(filename != null);

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
                FileModel.UpdateFrom(openFileResponse.Result);
                SettingsManager.CurrentDirectoryName = Path.GetDirectoryName(filename);

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
                    var decryptForDisplayResult = await Task.Run(() => _encryptionService.DecryptForDisplay(FileModel,
                        FileModel.VisualCryptText, _longRunningOperation.Context));
                    if (decryptForDisplayResult.IsSuccess)
                    {
                        // we were lucky, the password we have is correct!
                        FileModel.UpdateFrom(decryptForDisplayResult.Result); // do this before pushing the text to the editor
                        _eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
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
        public void CancelLongRunningOperation()
        {
            _longRunningOperation.Cancel();
        }

        void UpdateCanExecuteChanged()
        {
            // TODO: many more Commands, like Replace must be updated
            //_exportCommand.RaiseCanExecuteChanged();
            _encryptEditorContentsCommand.RaiseCanExecuteChanged();
            _decryptEditorContentsCommand.RaiseCanExecuteChanged();
        }



        #endregion

        #region SaveCommand

        DelegateCommand _saveCommand;

        public DelegateCommand SaveCommand
        {
            get { return CreateCommand(ref _saveCommand, ExecuteSaveCommand, () => FileModel.SaveEncoding != null); }
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
                if (FileModel.IsEncrypted && !isSaveAs && CheckFilenameForQuickSave())
                {
                    var response = _encryptionService.SaveEncryptedFile(FileModel);
                    if (!response.IsSuccess)
                        throw new Exception(response.Error);
                    FileModel.IsDirty = false;
                }
                // This is the case where we need a new filename and can then also 'just save'.
                else if (FileModel.IsEncrypted && (isSaveAs || !CheckFilenameForQuickSave()))
                {
                    string suggestedFilename = null;
                    if (isSaveAs)
                        suggestedFilename = FileModel.Filename;

                    var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
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
            if (isSaveAs || !CheckFilenameForQuickSave())
            {
                string suggestedFilename = null;
                if (isSaveAs)
                    suggestedFilename = FileModel.Filename;

                var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.VisualCrypt, DialogDirection.Save);
                if (!pickFileResult.Item1)
                    return;
                FileModel.Filename = pickFileResult.Item2;
            }
            // No we have password and filename, we can now encrypt and save in one step.
            // We will not replace FileManager.FileModel because we continue editing the same cleartext.
            string editorClearText = await EditorSendsTextAsync();

            using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationEncryptAndSave))
            {
                var encryptAndSaveFileResponse =
                    await
                        Task.Run(
                            () =>
                                _encryptionService.EncryptAndSaveFile(FileModel, editorClearText, new RoundsExponent(SettingsManager.EditorSettings.CryptographySettings.LogRounds), _longRunningOperation.Context));

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

        bool CheckFilenameForQuickSave()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SaveAsCommand

        DelegateCommand _saveAsCommand;

        public DelegateCommand SaveAsCommand
        {
            get
            {
                return CreateCommand(ref _saveAsCommand, ExecuteSaveAsCommand, () => FileModel.SaveEncoding != null);
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
                    FileModel.SaveEncoding.EncodingName);

                string suggestedFilename = FileModel.Filename.ReplaceCaseInsensitive(PortableConstants.DotVisualCrypt,
                    string.Empty);
                var pickFileResult = await PickFileAsync(suggestedFilename, DialogFilter.Text, DialogDirection.Save, title);
                if (pickFileResult.Item1)
                {
                    byte[] encodedTextBytes = FileModel.SaveEncoding.GetBytes(editorClearText);
                    File.WriteAllBytes(pickFileResult.Item2, encodedTextBytes);
                }
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        static async Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter,
            DialogDirection dialogDirection, string title = null)
        {
            throw new NotImplementedException();
            //FileDialog fileDialog;
            //if (dialogDirection == DialogDirection.Open)
            //    fileDialog = new OpenFileDialog();
            //else
            //    fileDialog = new SaveFileDialog();

            //if (title != null)
            //    fileDialog.Title = title;

            //if (!string.IsNullOrEmpty(suggestedFilename))
            //    fileDialog.FileName = suggestedFilename;

            //fileDialog.InitialDirectory = SettingsManager.CurrentDirectoryName;
            //if (diaglogFilter == DialogFilter.VisualCrypt)
            //{
            //    fileDialog.DefaultExt = Constants.VisualCryptDialogFilter_DefaultExt;
            //    fileDialog.Filter = Constants.VisualCryptDialogFilter;
            //}
            //else
            //{
            //    fileDialog.DefaultExt = Constants.TextDialogFilter_DefaultExt;
            //    fileDialog.Filter = Constants.TextDialogFilter;
            //}

            //var tcs = new TaskCompletionSource<Tuple<bool, string>>();

            //var okClicked = fileDialog.ShowDialog() == true;
            //var selectedFilename = fileDialog.FileName;

            //tcs.SetResult(new Tuple<bool, string>(okClicked, selectedFilename));
            //return await tcs.Task;
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
                                    _encryptionService.EncryptForDisplay(FileModel, textBufferContents, new RoundsExponent(SettingsManager.EditorSettings.CryptographySettings.LogRounds), _longRunningOperation.Context));
                    if (createEncryptedFileResponse.IsSuccess)
                    {
                        FileModel.UpdateFrom(createEncryptedFileResponse.Result);  // do this BEFORE pushing the text to the editor
                        _eventAggregator.GetEvent<EditorReceivesText>().Publish(createEncryptedFileResponse.Result.VisualCryptText);
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
                                    _encryptionService.DecryptForDisplay(FileModel, textBufferContents, _longRunningOperation.Context));
                    if (decryptForDisplayResult.IsSuccess)
                    {
                        FileModel.UpdateFrom(decryptForDisplayResult.Result); // do this BEFORE pushing the text to the editor
                        _eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
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



        #endregion

        #region private methods

        bool ConfirmToDiscardText()
        {
            return true;
            //if (FileModel.IsDirty)
            //    return
            //        (_messageBoxService.Show("Discard changes?", Constants.ProductName, MessageBoxButton.OKCancel,
            //            MessageBoxImage.Question) ==
            //         MessageBoxResult.OK);

            //return true;
        }

        #endregion

        #region ICleanup

        public void Cleanup()
        {
            _eventAggregator.GetEvent<EditorShouldCleanup>().Publish(null);
            _eventAggregator.GetEvent<EditorSendsText>().Unsubscribe(ExecuteEditorSendsTextCallback);
            _eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Unsubscribe(StatusBarModel.UpdateStatusBarText);
            _frameNavigation = null;
        }

        #endregion
    }
}
