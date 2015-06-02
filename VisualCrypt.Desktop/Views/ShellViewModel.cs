using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using MahApps.Metro;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.Events;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.Services;
using System.Threading.Tasks;

namespace VisualCrypt.Desktop.Views
{
    [Export]
    public class ShellViewModel : ViewModelBase
    {
        readonly IRegionManager _regionManager;
        readonly IEventAggregator _eventAggregator;
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;

        [ImportingConstructor]
        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            IMessageBoxService messageBoxService, IEncryptionService encryptionService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _messageBoxService = messageBoxService;
            _encryptionService = encryptionService;
            _eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Subscribe(OnEditorSendsStatusBarInfo);
            _eventAggregator.GetEvent<EditorSendsText>().Subscribe(OnEditorSendsText);
            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.Accents
                .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.AppThemes
                .Select(
                    a =>
                        new AppThemeMenuData()
                        {
                            Name = a.Name,
                            BorderColorBrush = a.Resources["BlackColorBrush"] as Brush,
                            ColorBrush = a.Resources["WhiteColorBrush"] as Brush
                        })
                .ToList();
        }

        public void Init()
        {
            CreateEditor();
            ExecuteNewCommand();
        }

        void OnEditorSendsText(EditorSendsText args)
        {
            if (args != null && args.Callback != null)
                args.Callback(args.Text);
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

        void ExecuteImportWithEncodingCommand()
        {
            try
            {
                if (!ConfirmToDiscardText())
                    return;

                var importEncoding = new ImportEncodingDialog
                {
                    WindowStyle = WindowStyle.ToolWindow,
                    Owner = Application.Current.MainWindow
                };
                
                if (importEncoding.ShowDialog() == true)
                {
                    if (!ConfirmToDiscardText())
                        return;

                    var selectedEncoding = importEncoding.SelectedEncodingInfo.GetEncoding();

                    string title = "Import With Encoding: {0})".FormatInvariant(selectedEncoding);
                    string filename = null;
                    if (TryPickFile(ref filename, DialogFilter.Text, DialogDirection.Open, title))
                    {
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
            get { return CreateCommand(ref _exportCommand, ExecuteExportCommand, () => !FileManager.FileModel.IsEncrypted); }
        }

        void ExecuteExportCommand()
        {
            try
            {
                _eventAggregator.GetEvent<EditorShouldSendText>().Publish(ExportCommandCallback);
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }


        void ExportCommandCallback(string clearText)
        {
            try
            {
                if (clearText == null)
                    throw new ArgumentNullException("clearText");

                string title = "Export Clear Text (Encoding: {0})".FormatInvariant(
                           FileManager.FileModel.SaveEncoding.EncodingName);

                string filename = FileManager.FileModel.Filename.ReplaceCaseInsensitive(Constants.DotVisualCrypt, string.Empty);

                if (TryPickFile(ref filename, DialogFilter.Text, DialogDirection.Save, title))
                {
                    byte[] encodedTextBytes = FileManager.FileModel.SaveEncoding.GetBytes(clearText);
                    File.WriteAllBytes(filename, encodedTextBytes);
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
                var process = new Process { StartInfo = { UseShellExecute = true, FileName = Constants.HelpUrl } };
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

        void ExecuteAboutCommand()
        {
            var aboutDialog = new AboutDialog { WindowStyle = WindowStyle.ToolWindow, Owner = Application.Current.MainWindow };
            aboutDialog.ShowDialog();
        }

        #endregion

        #region LogCommand

        public DelegateCommand LogCommand
        {
            get { return CreateCommand(ref _logCommand, ExecuteLogCommand, () => true); }
        }


        DelegateCommand _logCommand;


        void ExecuteLogCommand()
        {
            ServiceLocator.Current.GetInstance<LogWindow>().Show();
        }

        #endregion

        #region OpenCommand

        DelegateCommand _openCommand;

        public DelegateCommand OpenCommand
        {
            get { return CreateCommand(ref _openCommand, ExecuteOpenCommand, () => true); }
        }

        void ExecuteOpenCommand()
        {
            if (!ConfirmToDiscardText())
                return;

            string filename = null;
            if (TryPickFile(ref filename, DialogFilter.VisualCrypt, DialogDirection.Open))
            {
                OpenFileCommon(filename);
            }
        }


        void OpenFileCommon(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
                return;

            try
            {
                var openFileResponse = _encryptionService.OpenFile(filename);
                if (!openFileResponse.Success)
                {
                    _messageBoxService.ShowError(openFileResponse.Error);
                    return;
                }
                FileManager.FileModel = openFileResponse.Result;
                SettingsManager.CurrentDirectoryName = Path.GetDirectoryName(filename);

                if (FileManager.FileModel.IsEncrypted)
                    _eventAggregator.GetEvent<EditorReceivesText>().Publish(FileManager.FileModel.VisualCryptText);
                else
                    _eventAggregator.GetEvent<EditorReceivesText>().Publish(FileManager.FileModel.ClearTextContents);

                // if the loaded file was cleartext we are all done
                if (!FileManager.FileModel.IsEncrypted)
                    return;

                // if it's encrypted, check if we have SOME password
                // TODO: use async to make sure the contents loaded is already visible
                if (!PasswordManager.PasswordInfo.IsPasswordSet)
                {
                    bool result = ShowSetPasswordDialog(SetPasswordDialogMode.SetAndDecryptLoadedFile);
                    if (result == false)
                        return; // The user prefers to look at the cipher!
                }

            tryDecryptLoadFileWithCurrentPassword:
                // We have a password, but we don't know if it's the right one. We must try!
                var decryptForDisplayResult = _encryptionService.DecryptForDisplay(FileManager.FileModel,
                    FileManager.FileModel.VisualCryptText);
                if (decryptForDisplayResult.Success)
                {
                    // we were lucky, the password we have is correct!
                    FileManager.FileModel = decryptForDisplayResult.Result; // do this before pushing the text to the editor
                    _eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
                    UpdateCanExecuteChanged();
                    return; // exit from this goto-loop!
                }
                // As we tested that it's valid VisualCrypt in the open routine, we can assume we are here because
                // the password was wrong. So we ask the user again..:
                bool result2 = ShowSetPasswordDialog(SetPasswordDialogMode.CorrectPassword);
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


        public void OpenFileFromCommandLine(string[] args)
        {
            if (args == null)
                return;

            var commandline = string.Empty;

            // args is expected to hold one filename only
            // but can be segmented if it contains spaces.
            foreach (string a in args)
            {
                commandline += a;
                commandline += " ";
            }
            commandline = commandline.Trim();
            if (commandline.Length == 0)
                return;

            OpenFileCommon(commandline);
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
                    () => !FileManager.FileModel.IsEncrypted);
            }
        }


        void ExecuteEncryptEditorContentsCommand()
        {
            try
            {
                if (!PasswordManager.PasswordInfo.IsPasswordSet)
                {
                    bool result = ShowSetPasswordDialog(SetPasswordDialogMode.SetAndEncrypt);
                    if (result == false)
                        return;
                }
                _eventAggregator.GetEvent<EditorShouldSendText>().Publish(textBufferContents =>
                {
                    var createEncryptedFileResponse = _encryptionService.EncryptForDisplay(FileManager.FileModel, textBufferContents);
                    if (createEncryptedFileResponse.Success)
                    {
                        FileManager.FileModel = createEncryptedFileResponse.Result; // do this before pushing the text to the editor
                        _eventAggregator.GetEvent<EditorReceivesText>().Publish(createEncryptedFileResponse.Result.VisualCryptText);
                        UpdateCanExecuteChanged();
                    }
                    else
                    {
                        _messageBoxService.ShowError(createEncryptedFileResponse.Error);
                    }
                });
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
                    () => FileManager.FileModel.IsEncrypted);
            }
        }


        void ExecuteDecryptEditorContentsCommand()
        {
            try
            {
                if (!PasswordManager.PasswordInfo.IsPasswordSet)
                {
                    bool result = ShowSetPasswordDialog(SetPasswordDialogMode.SetAndDecrypt);
                    if (result == false)
                        return;
                }
                _eventAggregator.GetEvent<EditorShouldSendText>().Publish(textBufferContents =>
                {
                    var decryptForDisplayResult = _encryptionService.DecryptForDisplay(FileManager.FileModel, textBufferContents);
                    if (decryptForDisplayResult.Success)
                    {
                        FileManager.FileModel = decryptForDisplayResult.Result; // do this before pushing the text to the editor
                        _eventAggregator.GetEvent<EditorReceivesText>().Publish(decryptForDisplayResult.Result.ClearTextContents);
                        UpdateCanExecuteChanged();
                    }
                    else
                    {
                        _messageBoxService.ShowError(decryptForDisplayResult.Error);
                    }
                });
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        #endregion

        #region SaveCommand

        DelegateCommand _saveCommand;

        public DelegateCommand SaveCommand
        {
            get { return CreateCommand(ref _saveCommand, ExecuteSaveCommand, () => true); }
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
                    if (!response.Success)
                        throw new Exception(response.Error);
                    FileManager.FileModel.IsDirty = false;
                }
                // This is the case where we need a new filename and can then also 'just save'.
                else if (FileManager.FileModel.IsEncrypted && (isSaveAs || !FileManager.FileModel.CheckFilenameForQuickSave()))
                {
                    string filename = null;
                    if (isSaveAs)
                        filename = FileManager.FileModel.Filename;

                    if (TryPickFile(ref filename, DialogFilter.VisualCrypt, DialogDirection.Save))
                    {
                        FileManager.FileModel.Filename = filename;
                        var response = _encryptionService.SaveEncryptedFile(FileManager.FileModel);
                        if (!response.Success)
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
                bool result = ShowSetPasswordDialog(SetPasswordDialogMode.SetAndEncryptAndSave);
                if (result == false)
                    return;
            }
            // Then we must sure we have the file name:
            if (isSaveAs || !FileManager.FileModel.CheckFilenameForQuickSave())
            {
                string filename = null;
                if (!TryPickFile(ref filename, DialogFilter.VisualCrypt, DialogDirection.Save))
                    return;
                FileManager.FileModel.Filename = filename;
            }
            // No we have password and filename, we can now encrypt and save in one step.
            // We will not replace FileManager.FileModel because we continue editing the same cleartext.
            string clearTextBackup = null;
            string visualCryptTextSaved = null;
            _eventAggregator.GetEvent<EditorShouldSendText>().Publish(async textBufferContents =>
            {
                clearTextBackup = textBufferContents;
                var encryptAndSaveFileResponse = await Task.Run(()=>_encryptionService.EncryptAndSaveFile(FileManager.FileModel, textBufferContents));
                if (encryptAndSaveFileResponse.Success)
                {
                    visualCryptTextSaved = encryptAndSaveFileResponse.Result;
                }
                else
                {
                    throw new Exception(encryptAndSaveFileResponse.Error);
                }
                // We are done with successful saving. Show that we did encrypt the text:
                _eventAggregator.GetEvent<EditorReceivesText>().Publish(visualCryptTextSaved);
                await Task.Delay(2000);
                // Redisplay the clear text, to allow continue editing.
                FileManager.FileModel.IsDirty = false;

                _eventAggregator.GetEvent<EditorReceivesText>().Publish(clearTextBackup);

                UpdateCanExecuteChanged();
            });

           
        }

        string GetFilenameSafe(string pathString)
        {
            try
            {
                return Path.GetFileName(pathString);
            }
            catch (Exception)
            {
                return Constants.UntitledDotVisualCrypt;
            }
        }

        #endregion

        #region SaveAsCommand

        DelegateCommand _saveAsCommand;

        public DelegateCommand SaveAsCommand
        {
            get { return CreateCommand(ref _saveAsCommand, ExecuteSaveAsCommand, () => true); }
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
            get { return CreateCommand(ref _showSetPasswordDialogCommand, ExecuteShowSetPasswordDialogCommand, () => true); }
        }

        void ExecuteShowSetPasswordDialogCommand()
        {
            if (PasswordManager.PasswordInfo.IsPasswordSet)
                ShowSetPasswordDialog(SetPasswordDialogMode.Change);
            else
            {
                ShowSetPasswordDialog(SetPasswordDialogMode.Set);
            }
        }


        /// <summary>
        /// Returns true, if the user did positively set a password, otherwise always false;
        /// </summary>
        bool ShowSetPasswordDialog(SetPasswordDialogMode setPasswordDialogMode)
        {
            var setPassword = new SetPasswordDialog(setPasswordDialogMode, _messageBoxService, _encryptionService)
            {
                WindowStyle = WindowStyle.ToolWindow,
                Owner = Application.Current.MainWindow
            };
            var okClicked = setPassword.ShowDialog();

            return okClicked == true;
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
                if (!clearPasswordResponse.Success)
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

        void CreateEditor()
        {
            var mainRegion = _regionManager.Regions[RegionNames.EditorRegion];
            if (mainRegion == null)
                throw new InvalidOperationException(
                    "The region {0} is missing and has probably not been defined in Xaml.".FormatInvariant(
                        RegionNames.EditorRegion));

            var view = mainRegion.GetView(typeof(IEditor).Name) as IEditor;
            if (view == null)
            {
                view = ServiceLocator.Current.GetInstance<IEditor>();
                mainRegion.Add(view, typeof(IEditor).Name); // automatically activates the view
            }
            else
            {
                mainRegion.Activate(view);
            }
        }

        static bool TryPickFile(ref string filename, DialogFilter diaglogFilter, DialogDirection dialogDirection, string title = null)
        {
            FileDialog fileDialog;
            if (dialogDirection == DialogDirection.Open)
                fileDialog = new OpenFileDialog();
            else
                fileDialog = new SaveFileDialog();

            if (title != null)
                fileDialog.Title = title;

            if (!string.IsNullOrEmpty(filename))
                fileDialog.FileName = filename;

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
            if (fileDialog.ShowDialog() == true)
            {
                filename = fileDialog.FileName;
                return true;
            }
            return false;
        }

        #endregion

        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }
    }

    public class AccentColorMenuData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        ICommand _changeAccentCommand;
        public ICommand ChangeAccentCommand
        {
            get
            {
                return _changeAccentCommand ??
                       (_changeAccentCommand = new DelegateCommand<object>(DoChangeTheme, (x) => true));
            }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(this.Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }
}