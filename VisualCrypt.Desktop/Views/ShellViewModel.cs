using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
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
using VisualCrypt.Cryptography.Portable.Editor.Enums;

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
            IMessageBoxService messageBoxService,IEncryptionService encryptionService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _messageBoxService = messageBoxService;
            _encryptionService = encryptionService;
            _eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Subscribe(OnEditorSendsStatusBarInfo);
            _eventAggregator.GetEvent<EditorSendsText>().Subscribe(OnEditorSendsText);
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

        public Visibility TextBlockClearPasswordVisibility
        {
            get { return _textBlockClearPasswordVisibility; }
            set
            {
                if (_textBlockClearPasswordVisibility == value) return;
                _textBlockClearPasswordVisibility = value;
                OnPropertyChanged(() => TextBlockClearPasswordVisibility);
            }
        }

        Visibility _textBlockClearPasswordVisibility = Visibility.Collapsed;


        public string PasswordStatus
        {
            get
            {
                //if (FileManager.FileModel.IsPasswordPresent == false)
                //    return "Not Set";

                // Unicode Character 'BLACK CIRCLE' (U+25CF)
                return new string('\u25CF', 5);
            }
        }

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
            FileManager.FileModel = new CleartextFileModel();
            _eventAggregator.GetEvent<EditorReceivesText>().Publish(FileManager.FileModel.Contents);
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

                var importEncoding = new ImportEncoding
                {
                    WindowStyle = WindowStyle.ToolWindow,
                    Owner = Application.Current.MainWindow
                };
                var result = importEncoding.ShowDialog();
                if (result == true)
                {
                    var selectedEncoding = importEncoding.SelectedEncodingInfo.GetEncoding();
                    if (!ConfirmToDiscardText())
                        return;

                    var openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = SettingsManager.CurrentDirectoryName;
                    openFileDialog.DefaultExt = ".txt";
                    openFileDialog.Filter = "Text|*.txt|All Files|*.*";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        string importedString = File.ReadAllText(openFileDialog.FileName, selectedEncoding);
                        FileManager.FileModel = new CleartextFileModel(importedString, selectedEncoding,
                            Path.GetFileName(openFileDialog.FileName));
                        _eventAggregator.GetEvent<EditorReceivesText>().Publish(FileManager.FileModel.Contents);
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

        DelegateCommand _saveAsCommand;

        public DelegateCommand ExportCommand
        {
            get { return CreateCommand(ref _saveAsCommand, ExecuteExportCommand, () => true); }
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

                var saveFileDialog = new SaveFileDialog
                {
                    Title =
                        "Export Clear Text (Encoding: {0})".FormatInvariant(
                            FileManager.FileModel.SaveEncoding.EncodingName),
                    InitialDirectory = SettingsManager.CurrentDirectoryName,
                    FileName =
                        FileManager.FileModel.Filename.ReplaceCaseInsensitive(Constants.DotVisualCrypt, string.Empty),
                    DefaultExt = ".txt",
                    Filter = "Text|*.txt|All Files|*.*"
                };

                var result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    // Remember Directory after Export Clear Text?
                    // string fullPath = saveFileDialog.FileName;
                    // SettingsManager.CurrentDirectoryName = Path.GetDirectoryName(fullPath);
                    byte[] encodedTextBytes = FileManager.FileModel.SaveEncoding.GetBytes(clearText);

                    File.WriteAllBytes(saveFileDialog.FileName, encodedTextBytes);
                    // Reset IsDirty after Export Clear Text, too?
                    // FileManager.FileModel.IsDirty = true;
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
                var process = new Process {StartInfo = {UseShellExecute = true, FileName = Constants.HelpUrl}};
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
            var aboutDialog = new About {WindowStyle = WindowStyle.ToolWindow, Owner = Application.Current.MainWindow};
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
            ServiceLocator.Current.GetInstance<ModuleWindow>().Show();
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

            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = SettingsManager.CurrentDirectoryName;
            openFileDialog.DefaultExt = ".visualcrypt";
            openFileDialog.Filter = "VisualCrypt|*.visualcrypt; *.txt|Text|*.txt|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                OpenFileCommon(openFileDialog.FileName);
            }
        }

        #endregion

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

                _eventAggregator.GetEvent<EditorReceivesText>().Publish(FileManager.FileModel.Contents);

             


                if (FileManager.FileModel.IsEncrypted)
                {
                    //DecryptEditorContentsCommand.Execute();
                    //_eventAggregator.GetEvent<EditorShouldSendText>().Publish( s=>
                    //{
                    //    MessageBox.Show(s);
                    //    ;

                    //});
                }
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

        #region ShowSetPasswordDialogCommand

        DelegateCommand _showSetPasswordDialogCommand;

        public DelegateCommand ShowSetPasswordDialogCommand
        {
            get
            {
                return CreateCommand(ref _showSetPasswordDialogCommand, ExecuteShowSetPasswordDialogCommand, () => true);
            }
        }

        void ExecuteShowSetPasswordDialogCommand()
        {
            ShowSetPasswordDialog(SetPasswordDialogMode.Set);
        }


        /// <summary>
        /// Returns true, if the user did positively set a password, otherwise always false;
        /// </summary>
        bool ShowSetPasswordDialog(SetPasswordDialogMode setPasswordDialogMode)
        {
            var setPassword = new SetPassword(setPasswordDialogMode, _messageBoxService)
            {
                WindowStyle = WindowStyle.ToolWindow,
                Owner = Application.Current.MainWindow
            };
            var okClicked = setPassword.ShowDialog();
            if (true /*FileManager.FileModel.IsPasswordPresent*/)
                TextBlockClearPasswordVisibility = Visibility.Visible;
            /* else
                TextBlockClearPasswordVisibility = Visibility.Collapsed;*/
            OnPropertyChanged(() => PasswordStatus);
            return okClicked == true;
        }

        #endregion

        public bool CanExecuteClearPasswordCommand()
        {
            throw new NotImplementedException();
        }

        public void ExecuteClearPasswordCommand()
        {
            throw new NotImplementedException();
        }

        #region Private Methods

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

            var view = mainRegion.GetView(typeof (IEditor).Name) as IEditor;
            if (view == null)
            {
                view = ServiceLocator.Current.GetInstance<IEditor>();
                mainRegion.Add(view, typeof (IEditor).Name); // automatically activates the view
            }
            else
            {
                mainRegion.Activate(view);
            }
        }

        #endregion
    }
}