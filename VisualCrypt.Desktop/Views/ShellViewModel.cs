﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
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

        DelegateCommand _exportCommand;

        public DelegateCommand ExportCommand
        {
            get { return CreateCommand(ref _exportCommand, ExecuteExportCommand, () => true); }
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
            var aboutDialog = new About { WindowStyle = WindowStyle.ToolWindow, Owner = Application.Current.MainWindow };
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

        #endregion

        #region EncryptEditorContentsCommand

        DelegateCommand _encryptEditorContentsCommand;

        public DelegateCommand EncryptEditorContentsCommand
        {
            get
            {
                return CreateCommand(ref _encryptEditorContentsCommand, ExecuteEncryptEditorContentsCommand, () => !FileManager.FileModel.IsEncrypted);
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
                // Clear Undo stack and disable Undo
                //_mainWindow.TextBox1.IsUndoEnabled = false;

                // do the encryption
               // var encryptResponse = ModelState.Transient.FileModel.Encrypt(new ClearText(_mainWindow.TextBox1.Text));

                //if (!encryptResponse.Success)
                //    throw new Exception(encryptResponse.Error);

                //SetTextAndUpdateAllWithoutUndo(encryptResponse.Result.Value);

            }
            catch (Exception e)
            {

                //MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }
        }

        #endregion

        #region DecryptEditorContentsCommand

        DelegateCommand _decryptEditorContentsCommand;

        public DelegateCommand DecryptEditorContentsCommand
        {
            get
            {
                return CreateCommand(ref _decryptEditorContentsCommand, ExecuteDecryptEditorContentsCommand, () => FileManager.FileModel.IsEncrypted);
            }
        }


        void ExecuteDecryptEditorContentsCommand()
        {
            try
            {
                //var decodeResponse =
                //    _visualCryptAPI.TryDecodeVisualCryptText(new VisualCryptText(_mainWindow.TextBox1.Text));

                //if (!decodeResponse.Success)
                //{
                //    MessageBoxService.ShowError(decodeResponse.Error);
                //    return;
                //}
                //if (!ModelState.Transient.FileModel.IsPasswordPresent)
                //{
                //    if (!ShowSetPasswordDialog(SetPasswordDialogMode.SetAndDecrypt))
                //        return;
                //    ExecuteDecryptEditorContentsCommand(); // loop!
                //}
                //else
                //{
                //    var decrpytResponse = ModelState.Transient.FileModel.Decrypt(decodeResponse.Result);
                //    if (!decrpytResponse.Success)
                //    {
                //        MessageBoxService.ShowError(decrpytResponse.Error);
                //        return;
                //    }
                //    SetTextAndUpdateAllWithoutUndo(decrpytResponse.Result.Value);
                //}
            }
            catch (Exception e)
            {
                //MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }
        }

        #endregion

        #region SaveCommand

        DelegateCommand _saveCommand;

        public DelegateCommand SaveCommand
        {
            get { return CreateCommand(ref _saveCommand, ExecuteSaveCommand, CanExecuteSaveCommand); }
        }

        void ExecuteSaveCommand()
        {
            //try
            //{
            //    // TODO: THIS IS THE OLD WAY!!!
            //    byte[] encodedTextBytes = ModelState.Transient.FileModel.SaveEncoding.GetBytes(_mainWindow.TextBox1.Text);

            //    string fullPath = Path.Combine(ModelState.Transient.CurrentDirectoryName, ModelState.Transient.FileModel.Filename);
            //    File.WriteAllBytes(fullPath, encodedTextBytes);

            //    ModelState.Transient.FileModel.IsDirty = false;
            //    UpdateWindowTitle();
            //    UpdateStatusBar();
            //    SaveCommand.RaiseCanExecuteChanged();
            //}
            //catch (Exception e)
            //{
            //    MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            //}
        }

        bool CanExecuteSaveCommand()
        {
            //if (!ModelState.Transient.FileModel.IsDirty && ModelState.Transient.FileModel.IsEncrypted
            //    && ModelState.Transient.FileModel.IsFilenamePresent)
            //    return true;
            return false;
        }

        #endregion

        #region SaveAsCommand

        DelegateCommand _saveAsCommand;

        public DelegateCommand SaveAsCommand
        {
            get { return CreateCommand(ref _saveAsCommand, ExecuteSaveAsCommand, CanExecuteSaveAsCommand); }
        }

        void ExecuteSaveAsCommand()
        {

            //var saveFileDialog = new SaveFileDialog
            //{
            //    InitialDirectory = ModelState.Transient.CurrentDirectoryName ?? ModelState.Defaults.DefaultDirectoryName,
            //    FileName = ModelState.Transient.FileModel.Filename ?? Defaults.UntitledDotVisualCrypt,
            //    DefaultExt = ".visualcrypt",
            //    Filter = "VisualCrypt|*.visualcrypt; *.txt|Text|*.txt|All Files|*.*"
            //};

            //var result = saveFileDialog.ShowDialog();

            //if (result == true)
            //{
            //    string fullPath = saveFileDialog.FileName;

            //    try
            //    {
            //        ModelState.Transient.CurrentDirectoryName = Path.GetDirectoryName(fullPath);


            //        byte[] encodedTextBytes = ModelState.Transient.FileModel.SaveEncoding.GetBytes(_mainWindow.TextBox1.Text);

            //        File.WriteAllBytes(fullPath, encodedTextBytes);

            //        ModelState.Transient.FileModel.IsDirty = false;
            //        UpdateWindowTitle();
            //        UpdateStatusBar();
            //        SaveCommand.RaiseCanExecuteChanged();
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            //    }
            //}
        }

        bool CanExecuteSaveAsCommand()
        {
            return true;
        }

        #endregion

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
            var setPassword = new SetPassword(setPasswordDialogMode, _messageBoxService, _encryptionService)
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

        #endregion
    }
}