using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;
using VisualCrypt.Language;
using VisualCrypt.Windows.Events;
using VisualCrypt.Windows.Infrastructure;
using VisualCrypt.Windows.Models;
using VisualCrypt.Windows.Services;
using VisualCrypt.Windows.Static;
using VisualCrypt.Windows.V2;

namespace VisualCrypt.Windows.Pages
{
    class MainPageViewModel : ViewModelBase
    {
        readonly IEncryptionService _encryptionService = new EncryptionService();
        readonly IMessageBoxService _messageBoxService;
        readonly IEventAggregator _eventAggregator = SharedInstances.EventAggregator;
        LongRunningOperation _longRunningOperation;
        Frame _frame;
        TextBox _textBox1;

        public void OnViewInitialized(Frame frame, TextBox textBox1)
        {
            _frame = frame;
            _textBox1 = textBox1;
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e == null || e.Parameter == null || e.Parameter is FilesPageCommandArgs == false)
                throw new ArgumentException("Invalid NavigationEventArgs passed to MainPageViewModel.");

            var command = (FilesPageCommandArgs)e.Parameter;
            switch (command.FilesPageCommand)
            {
                case FilesPageCommand.New:
                    NewCommand.Execute();
                    break;
                case FilesPageCommand.Open:
                    OpenCommand.Execute(command.FileReference);
                    break;
                default:
                    throw new InvalidOperationException($"Unknwon command {command}");
            }
        }

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
            _textBox1.Text = FileManager.FileModel.ClearTextContents;
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
            _frame.Navigate(typeof (FilesPage));
        }

        #endregion

        #region OpenCommand

        DelegateCommand<FileReference> OpenCommand => CreateCommand(ref _openCommand, ExecuteOpenCommand, arg => true);
        DelegateCommand<FileReference> _openCommand;

        async void ExecuteOpenCommand(FileReference fileReference)
        {
            // _textBox1.Text = fileReference.Filename;

            var setPasswordResponse = _encryptionService.SetPassword("bla");
            if (!setPasswordResponse.IsSuccess)
            {
                PasswordManager.PasswordInfo.IsPasswordSet = false;
                _messageBoxService.ShowError(setPasswordResponse.Error);
                return;
            }
            PasswordManager.PasswordInfo.IsPasswordSet = true;

            await OpenFileCommon(fileReference.DirectoryName);
        }

        async Task OpenFileCommon(string filename)
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
                using (_longRunningOperation = StartLongRunnungOperation(Loc.Strings.operationDecryptOpenedFile))
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
                    if (decryptForDisplayResult.IsCanceled)
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

        LongRunningOperation StartLongRunnungOperation(string description)
        {
            FileManager.ShowWorkingBar(description);

            Action<EncryptionProgress> updateProgressBar = encryptionProgress =>
            {
                FileManager.BindableFileInfo.ProgressPercent = encryptionProgress.Percent;
                FileManager.BindableFileInfo.ProgressMessage = encryptionProgress.Message;
            };

            var switchBackToPreviousBar = FileManager.FileModel.IsEncrypted
                ? (Action)FileManager.ShowEncryptedBar
                : FileManager.ShowPlainTextBar;

            return new LongRunningOperation(updateProgressBar, switchBackToPreviousBar);
        }
        public void CancelLongRunningOperation()
        {
            _longRunningOperation.Cancel();
        }

        private void UpdateCanExecuteChanged()
        {
            
        }

        private Task<bool> SetPasswordAsync(SetPasswordDialogMode correctPassword)
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);
            return tcs.Task;
        }

        #endregion

        #region private methods

        bool ConfirmToDiscardText()
        {
            return true;
            //if (FileManager.FileModel.IsDirty)
            //    return
            //        (_messageBoxService.Show("Discard changes?", Constants.ProductName, MessageBoxButton.OKCancel,
            //            MessageBoxImage.Question) ==
            //         MessageBoxResult.OK);

            //return true;
        }

        #endregion
    }
}
