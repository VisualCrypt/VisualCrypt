using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisualCrypt.Desktop.Features;
using VisualCrypt.Desktop.Lib;
using VisualCrypt.Desktop.Models.Printing;
using VisualCrypt.Desktop.State;
using VisualCrypt.Net.APIV2.Implementations;
using VisualCrypt.Portable.APIV2.DataTypes;
using VisualCrypt.Portable.APIV2.Implementations;
using VisualCrypt.Portable.APIV2.Interfaces;
using VisualCrypt.Portable.Editor.Enums;
using VisualCrypt.Portable.Tools;


namespace VisualCrypt.Desktop.Views
{
    public sealed class MainWindowViewModel : ViewModelBase
    {

        readonly IVisualCryptAPIV2 _visualCryptAPI;
        readonly MainWindow _mainWindow;
        readonly FindReplaceViewModel _findReplaceDialogViewModel;
        readonly GoToViewModel _goToWindowViewModel;
        readonly SpellCheck _spellCheck;
        public IMessageBoxService MessageBoxService;



        public MainWindowViewModel(MainWindow mainWindow)
        {
            _visualCryptAPI = new VisualCryptAPIV2(new CoreAPIV2_Net4());

            _mainWindow = mainWindow;
            _findReplaceDialogViewModel = new FindReplaceViewModel(_mainWindow.TextBox1, FindNextCommand, FindPreviousCommand);
            _goToWindowViewModel = new GoToViewModel(_mainWindow.TextBox1);

            _spellCheck = _mainWindow.TextBox1.SpellCheck;
            _spellCheck.SpellingReform = SpellingReform.Postreform;
        }

        public void OnMainWindowInitialized()
        {
            ModelState.Init();

            ModelState.Transient.FileModel = new FileModel();

            ApplySettings();
            Zoom100Command.Execute();
            SetTextAndUpdateAllWithoutUndo(string.Empty);
        }


        public string WindowTitleText
        {
            get { return _windowTitleText; }
            set
            {
                if (_windowTitleText == value) return;
                _windowTitleText = value;
                RaisePropertyChanged(() => WindowTitleText);
            }
        }
        string _windowTitleText;


        public string StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                if (_statusBarText == value) return;
                _statusBarText = value;
                RaisePropertyChanged(() => StatusBarText);
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
                RaisePropertyChanged(() => TextBlockClearPasswordVisibility);
            }
        }
        Visibility _textBlockClearPasswordVisibility = Visibility.Collapsed;




        public string PasswordStatus
        {
            get
            {
                if (ModelState.Transient.FileModel.IsPasswordPresent == false)
                    return "Not Set";

                // Unicode Character 'BLACK CIRCLE' (U+25CF)
                return new string('\u25CF', 5);
            }
        }

        #region ClearPasswordCommand

        public DelegateCommand ClearPasswordCommand
        {
            get { return CreateCommand(ref _clearPasswordCommand, ExecuteClearPasswordCommand, () => ModelState.Transient.FileModel.IsPasswordPresent); }
        }
        DelegateCommand _clearPasswordCommand;

        void ExecuteClearPasswordCommand()
        {
            ModelState.Transient.FileModel.ClearPassword();

            RaisePropertyChanged(() => PasswordStatus);
            TextBlockClearPasswordVisibility = Visibility.Collapsed;
        }

        #endregion

        #region ExitCommand


        public DelegateCommand<CancelEventArgs> ExitCommand
        {
            get { return CreateCommand(ref _exitCommand, ExecuteExitCommand, e => true); }
        }
        DelegateCommand<CancelEventArgs> _exitCommand;

        void ExecuteExitCommand(CancelEventArgs e)
        {

            bool isInvokedFromWindowCloseEvent = e != null;

            if (isInvokedFromWindowCloseEvent)
            {
                if (ConfirmToDiscardText())
                    return;
                e.Cancel = true;
            }
            else
            {
                if (ConfirmToDiscardText())
                {
                    Application.Current.Shutdown();
                }
            }
        }


        #endregion

        #region TextChangedCommand


        public DelegateCommand<TextChangedEventArgs> TextChangedCommand
        {
            get
            {
                return CreateCommand(ref _textChangedCommand, ExecuteTextChangedCommand, e => true);
            }
        }
        DelegateCommand<TextChangedEventArgs> _textChangedCommand;

        void ExecuteTextChangedCommand(TextChangedEventArgs e)
        {
            ModelState.Transient.FileModel.IsDirty = true;
            RaiseAllCanExecuteChanged();
            if ((IsStatusBarChecked))
                UpdateStatusBar();
        }


        #endregion

        #region SelectionChangedCommand

        public DelegateCommand<RoutedEventArgs> SelectionChangedCommand
        {
            get
            {
                return CreateCommand(ref _selectionChangedCommand, ExecuteSelectionChangedCommand, e => true);
            }
        }
        DelegateCommand<RoutedEventArgs> _selectionChangedCommand;

        void ExecuteSelectionChangedCommand(RoutedEventArgs args)
        {
            if ((IsStatusBarChecked))
                UpdateStatusBar();
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

            ModelState.Transient.FileModel = new FileModel();

            ClearEditorAndUndoStack();
            UpdateWindowTitle();
            UpdateStatusBar();
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
            openFileDialog.InitialDirectory = ModelState.Transient.CurrentDirectoryName ?? ModelState.Defaults.DefaultDirectoryName;
            openFileDialog.DefaultExt = ".visualcrypt";
            openFileDialog.Filter = "VisualCrypt|*.visualcrypt; *.txt|Text|*.txt|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                OpenFileCommon(openFileDialog.FileName);
            }
        }

        #endregion

        #region EncryptEditorContentsCommand

        DelegateCommand _encryptEditorContentsCommand;

        public DelegateCommand EncryptEditorContentsCommand
        {
            get
            {
                return CreateCommand(ref _encryptEditorContentsCommand, ExecuteEncryptEditorContentsCommand, () => !ModelState.Transient.FileModel.IsEncrypted);
            }
        }


        void ExecuteEncryptEditorContentsCommand()
        {
            try
            {

                if (!ModelState.Transient.FileModel.IsPasswordPresent)
                {
                    bool result = ShowSetPasswordDialog(SetPasswordDialogMode.SetAndEncrypt);
                    if (result == false)
                        return;
                }
                // Clear Undo stack and disable Undo
                _mainWindow.TextBox1.IsUndoEnabled = false;

                // do the encryption
                var encryptResponse = ModelState.Transient.FileModel.Encrypt(new ClearText(_mainWindow.TextBox1.Text));

                if (!encryptResponse.Success)
                    throw new Exception(encryptResponse.Error);

                SetTextAndUpdateAllWithoutUndo(encryptResponse.Result.Value);

            }
            catch (Exception e)
            {

                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }
        }

        #endregion

        #region DecryptEditorContentsCommand

        DelegateCommand _decryptEditorContentsCommand;

        public DelegateCommand DecryptEditorContentsCommand
        {
            get
            {
                return CreateCommand(ref _decryptEditorContentsCommand, ExecuteDecryptEditorContentsCommand, () => ModelState.Transient.FileModel.IsEncrypted);
            }
        }


        void ExecuteDecryptEditorContentsCommand()
        {
            try
            {
                var decodeResponse =
                    _visualCryptAPI.TryDecodeVisualCryptText(new VisualCryptText(_mainWindow.TextBox1.Text));

                if (!decodeResponse.Success)
                {
                    MessageBoxService.ShowError(decodeResponse.Error);
                    return;
                }
                if (!ModelState.Transient.FileModel.IsPasswordPresent)
                {
                    if (!ShowSetPasswordDialog(SetPasswordDialogMode.SetAndDecrypt))
                        return;
                    ExecuteDecryptEditorContentsCommand(); // loop!
                }
                else
                {
                    var decrpytResponse = ModelState.Transient.FileModel.Decrypt(decodeResponse.Result);
                    if (!decrpytResponse.Success)
                    {
                        MessageBoxService.ShowError(decrpytResponse.Error);
                        return;
                    }
                    SetTextAndUpdateAllWithoutUndo(decrpytResponse.Result.Value);
                }
            }
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
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
            try
            {
                // TODO: THIS IS THE OLD WAY!!!
                byte[] encodedTextBytes = ModelState.Transient.FileModel.SaveEncoding.GetBytes(_mainWindow.TextBox1.Text);

                string fullPath = Path.Combine(ModelState.Transient.CurrentDirectoryName, ModelState.Transient.FileModel.Filename);
                File.WriteAllBytes(fullPath, encodedTextBytes);

                ModelState.Transient.FileModel.IsDirty = false;
                UpdateWindowTitle();
                UpdateStatusBar();
                SaveCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }
        }

        bool CanExecuteSaveCommand()
        {
            if (!ModelState.Transient.FileModel.IsDirty && ModelState.Transient.FileModel.IsEncrypted
                && ModelState.Transient.FileModel.IsFilenamePresent)
                return true;
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

            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = ModelState.Transient.CurrentDirectoryName ?? ModelState.Defaults.DefaultDirectoryName,
                FileName = ModelState.Transient.FileModel.Filename ?? Defaults.UntitledDotVisualCrypt,
                DefaultExt = ".visualcrypt",
                Filter = "VisualCrypt|*.visualcrypt; *.txt|Text|*.txt|All Files|*.*"
            };

            var result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string fullPath = saveFileDialog.FileName;

                try
                {
                    ModelState.Transient.CurrentDirectoryName = Path.GetDirectoryName(fullPath);


                    byte[] encodedTextBytes = ModelState.Transient.FileModel.SaveEncoding.GetBytes(_mainWindow.TextBox1.Text);

                    File.WriteAllBytes(fullPath, encodedTextBytes);

                    ModelState.Transient.FileModel.IsDirty = false;
                    UpdateWindowTitle();
                    UpdateStatusBar();
                    SaveCommand.RaiseCanExecuteChanged();
                }
                catch (Exception e)
                {
                    MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
                }
            }
        }

        bool CanExecuteSaveAsCommand()
        {
            return true;
        }

        #endregion

        #region WordWrapCommand

        public DelegateCommand WordWrapCommand
        {
            get { return CreateCommand(ref _wordWrapCommand, ExecuteWordWrapCommand, () => true); }
        }
        DelegateCommand _wordWrapCommand;

        void ExecuteWordWrapCommand()
        {
            if (IsWordWrapChecked)
                IsWordWrapChecked = false;
            else
                IsWordWrapChecked = true;
        }



        public bool IsWordWrapChecked
        {
            get { return ModelState.EditorState.IsWordWrapChecked; }
            set
            {
                if (ModelState.EditorState.IsWordWrapChecked != value)
                {
                    ModelState.EditorState.IsWordWrapChecked = value;
                    RaisePropertyChanged(() => IsWordWrapChecked);
                    RaisePropertyChanged(() => TextWrapping);
                    ModelState.SaveSettings();
                    UpdateStatusBar();
                }
            }
        }

        public TextWrapping TextWrapping
        {
            get
            {
                if (ModelState.EditorState.IsWordWrapChecked)
                    return TextWrapping.Wrap;
                return TextWrapping.NoWrap;
            }
        }

        #endregion

        #region SpellCheckCommand

        public DelegateCommand SpellCheckCommand
        {
            get { return CreateCommand(ref _spellCheckCommand, ExecuteSpellCheckCommand, () => true); }
        }
        DelegateCommand _spellCheckCommand;

        void ExecuteSpellCheckCommand()
        {
            if (IsSpellCheckingChecked)
                IsSpellCheckingChecked = false;
            else
                IsSpellCheckingChecked = true;
        }




        public bool IsSpellCheckingChecked
        {
            get { return ModelState.EditorState.IsSpellCheckingChecked; }
            set
            {

                ModelState.EditorState.IsSpellCheckingChecked = value;
                _spellCheck.IsEnabled = value;
                RaisePropertyChanged(() => IsSpellCheckingChecked);

                ModelState.SaveSettings();


            }
        }



        #endregion

        #region StatusBarCommand

        public DelegateCommand StatusBarCommand
        {
            get { return CreateCommand(ref _statusBarCommand, ExecuteStatusBarCommand, () => true); }
        }
        DelegateCommand _statusBarCommand;

        void ExecuteStatusBarCommand()
        {
            if (IsStatusBarChecked)
                IsStatusBarChecked = false;
            else
                IsStatusBarChecked = true;
        }



        public bool IsStatusBarChecked
        {
            get
            {
                return ModelState.EditorState.IsStatusBarChecked;
            }
            set
            {
                if (ModelState.EditorState.IsStatusBarChecked != value)
                {
                    ModelState.EditorState.IsStatusBarChecked = value;
                    RaisePropertyChanged(() => IsStatusBarChecked);
                    RaisePropertyChanged(() => StatusBarVisibility);
                    ModelState.SaveSettings();
                    UpdateStatusBar();
                }
            }
        }

        public Visibility StatusBarVisibility
        {
            get
            {
                if (ModelState.EditorState.IsStatusBarChecked)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        #endregion

        #region ZoomCommands

        public DelegateCommand ZoomInCommand
        {
            get { return CreateCommand(ref _zoomInCommand, ExecuteZoomInCommand, () => _mainWindow.TextBox1.FontSize < 999); }
        }
        DelegateCommand _zoomInCommand;

        void ExecuteZoomInCommand()
        {
            _mainWindow.TextBox1.FontSize *= 1.05;
            RaisePropertyChanged(() => Is100Checked);
            RaisePropertyChanged(() => ZoomLevelText);
            Zoom100Command.RaiseCanExecuteChanged();
            ZoomInCommand.RaiseCanExecuteChanged();
            ZoomOutCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand ZoomOutCommand
        {
            get { return CreateCommand(ref _zoomOutCommand, ExecuteZoomOutCommand, () => _mainWindow.TextBox1.FontSize > 1); }
        }
        DelegateCommand _zoomOutCommand;

        void ExecuteZoomOutCommand()
        {
            _mainWindow.TextBox1.FontSize *= 1 / 1.05;

            RaisePropertyChanged(() => Is100Checked);
            RaisePropertyChanged(() => ZoomLevelText);
            Zoom100Command.RaiseCanExecuteChanged();
            ZoomInCommand.RaiseCanExecuteChanged();
            ZoomOutCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand Zoom100Command
        {
            get { return CreateCommand(ref _zoom100Command, ExecuteZoom100Command, () => !Is100Checked); }
        }
        DelegateCommand _zoom100Command;

        void ExecuteZoom100Command()
        {
            _mainWindow.TextBox1.FontSize = ModelState.FontSettings.FontSize;

            RaisePropertyChanged(() => Is100Checked);
            RaisePropertyChanged(() => ZoomLevelText);
            Zoom100Command.RaiseCanExecuteChanged();
            ZoomInCommand.RaiseCanExecuteChanged();
            ZoomOutCommand.RaiseCanExecuteChanged();
        }



        public bool Is100Checked
        {
            get { return Math.Abs(((_mainWindow.TextBox1.FontSize / ModelState.FontSettings.FontSize) * 100) - 100) < 0.1; }
        }

        public string ZoomLevelText
        {
            get
            {
                var zoomLevel = (int)((_mainWindow.TextBox1.FontSize / ModelState.FontSettings.FontSize) * 100);
                var zoomLevelText = "_Zoom (" + zoomLevel + "%)";
                return zoomLevelText;
            }
        }


        #endregion

        #region PrintCommand

        DelegateCommand _printCommand;
        public DelegateCommand PrintCommand
        {
            get { return CreateCommand(ref _printCommand, ExecutePrintCommand, CanExecutePrintCommand); }
        }

        void ExecutePrintCommand()
        {
            try
            {
                var doc = Printer.ConvertToFlowDocument(_mainWindow.TextBox1.Text);
                Printer.PrintFlowDocument(doc, ModelState.PageSettings.Margin);
            }
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }
        }







        bool CanExecutePrintCommand()
        {
            return true;
        }

        #endregion

        #region FindCommand



        public DelegateCommand FindCommand
        {
            get { return CreateCommand(ref _findCommand, ExecuteFindCommand, () => _mainWindow.TextBox1.Text.Length > 0); }
        }
        DelegateCommand _findCommand;

        void ExecuteFindCommand()
        {

            _findReplaceDialogViewModel.TabControlSelectedIndex = 0;
            var fr = Application.Current.Windows.OfType<FindReplace>().FirstOrDefault();
            if (fr != null)
            {
                fr.Activate();
                fr.TextBoxFindFindString.SelectAll();
                fr.TextBoxFindFindString.Focus();
                return;
            }

            var findReplaceDialog = new FindReplace(_findReplaceDialogViewModel)
            {
                WindowStyle = WindowStyle.ToolWindow,
                Owner = _mainWindow
            };

            findReplaceDialog.Show();
            _findReplaceDialogViewModel.MessageBoxService = new MessageBoxService(findReplaceDialog);

        }

        #endregion

        #region FindNextCommand



        public DelegateCommand FindNextCommand
        {
            get { return CreateCommand(ref _findNextCommand, ExecuteFindNextCommand, CanExecuteFindNextCommand); }
        }
        DelegateCommand _findNextCommand;

        void ExecuteFindNextCommand()
        {

            var oldDirection = _findReplaceDialogViewModel.SearchOptions.SearchUp;
            _findReplaceDialogViewModel.SearchOptions.SearchUp = false;
            _findReplaceDialogViewModel.FindNextCommand.Execute();
            _findReplaceDialogViewModel.SearchOptions.SearchUp = oldDirection;
        }

        bool CanExecuteFindNextCommand()
        {
            return _findReplaceDialogViewModel.FindNextCommand.CanExecute() &&
                   !string.IsNullOrEmpty(_findReplaceDialogViewModel.FindString);
        }


        #endregion

        #region FindPreviousCommand



        public DelegateCommand FindPreviousCommand
        {
            get { return CreateCommand(ref _findPreviousCommand, ExecuteFindPreviousCommand, CanExecuteFindPreviousCommand); }
        }
        DelegateCommand _findPreviousCommand;

        void ExecuteFindPreviousCommand()
        {
            var oldDirection = _findReplaceDialogViewModel.SearchOptions.SearchUp;
            _findReplaceDialogViewModel.SearchOptions.SearchUp = true;
            _findReplaceDialogViewModel.FindNextCommand.Execute();
            _findReplaceDialogViewModel.SearchOptions.SearchUp = oldDirection;
        }

        bool CanExecuteFindPreviousCommand()
        {
            return _findReplaceDialogViewModel.FindNextCommand.CanExecute() &&
                   !string.IsNullOrEmpty(_findReplaceDialogViewModel.FindString);
        }


        #endregion

        #region ReplaceCommand

        public DelegateCommand ReplaceCommand
        {
            get
            {
                return CreateCommand(ref _replaceCommand, ExecuteReplaceCommand, () => _mainWindow.TextBox1.Text.Length > 0);
            }
        }
        DelegateCommand _replaceCommand;

        void ExecuteReplaceCommand()
        {
            _findReplaceDialogViewModel.TabControlSelectedIndex = 1;

            var fr = Application.Current.Windows.OfType<FindReplace>().FirstOrDefault();
            if (fr != null)
            {
                fr.Activate();
                fr.TextBoxReplaceString.SelectAll();
                fr.TextBoxReplaceString.Focus();
                return;
            }

            var findReplaceDialog = new FindReplace(_findReplaceDialogViewModel)
            {
                WindowStyle = WindowStyle.ToolWindow,
                Owner = _mainWindow
            };

            findReplaceDialog.Show();
            _findReplaceDialogViewModel.MessageBoxService = new MessageBoxService(findReplaceDialog);

        }

        #endregion

        #region DeleteLineCommand

        public DelegateCommand DeleteLineCommand
        {
            get
            {
                return CreateCommand(ref _deleteLineCommand, ExecuteDeleteLineCommand, () => true);
            }
        }
        DelegateCommand _deleteLineCommand;

        void ExecuteDeleteLineCommand()
        {
            try
            {
                var currentLineIndex = GetCurrentLineIndex();

                // 1.) Get col in line to delete
                var currentColIndex = GetColIndex(currentLineIndex);

                // 2.) Is there a next line?
                var isNextLine = _mainWindow.TextBox1.LineCount > currentLineIndex + 1;

                var currentLineLenght = _mainWindow.TextBox1.GetLineLength(currentLineIndex);
                if (currentLineLenght < 1)
                    return;


                var targetColIndex = 0;
                if (isNextLine)
                {
                    var nextLineLenght = _mainWindow.TextBox1.GetLineLength(currentLineIndex + 1);
                    var nextLineIsAtLeastAsLongAsCurrentLine = nextLineLenght >= currentLineLenght;

                    if (nextLineIsAtLeastAsLongAsCurrentLine)
                    {
                        targetColIndex = currentColIndex;
                    }
                    else
                    {
                        targetColIndex = _mainWindow.TextBox1.GetLineLength(currentLineIndex + 1) - 2;
                        if (targetColIndex < 0)
                            targetColIndex = 0;
                    }
                }
                // targetColIndex stays 0 if there is no next line!


                var firstPos = _mainWindow.TextBox1.GetCharacterIndexFromLineIndex(currentLineIndex);
                _mainWindow.TextBox1.Text = _mainWindow.TextBox1.Text.Remove(firstPos, currentLineLenght);



                var newPos = _mainWindow.TextBox1.GetCharacterIndexFromLineIndex(currentLineIndex) + targetColIndex;
                _mainWindow.TextBox1.CaretIndex = newPos;
            }
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }

        }



        #endregion

        #region GoToCommand

        public DelegateCommand GoToCommand
        {
            get
            {
                return CreateCommand(ref _goToCommand, ExecuteGoToCommand, CanExecuteGoToCommand);
            }
        }
        DelegateCommand _goToCommand;

        void ExecuteGoToCommand()
        {
            if (Application.Current.Windows.OfType<GoTo>().Any())
                return;

            var goToWindow = new GoTo(_goToWindowViewModel)
            {
                WindowStyle = WindowStyle.ToolWindow,
                Owner = _mainWindow
            };

            goToWindow.Show();
            goToWindow.SelectAll();
            _goToWindowViewModel.MessageBoxService = new MessageBoxService(goToWindow);
        }

        bool CanExecuteGoToCommand()
        {
            return _mainWindow.TextBox1.LineCount > 1;

        }

        #endregion

        #region DateCommand

        public DelegateCommand DateCommand
        {
            get
            {
                return CreateCommand(ref _dateCommand, ExecuteDateCommand, () => true);
            }
        }
        DelegateCommand _dateCommand;

        DateTime? _datePasted;


        void ExecuteDateCommand()
        {

            if (_datePasted.HasValue && DateTime.Now - _datePasted.Value < TimeSpan.FromSeconds(3))
            {
                var oldCaretIndex = _mainWindow.TextBox1.CaretIndex;
                var timeStringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
                var timeString = " " + DateTime.Now.ToString(timeStringPattern);
                _mainWindow.TextBox1.Text = _mainWindow.TextBox1.Text.Insert(_mainWindow.TextBox1.CaretIndex, timeString);
                _mainWindow.TextBox1.CaretIndex = oldCaretIndex + 1 + timeString.Length;
                _datePasted = null;
            }
            else
            {
                var oldCaretIndex = _mainWindow.TextBox1.CaretIndex;
                var datetringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                var dateString = DateTime.Now.ToString(datetringPattern);
                _mainWindow.TextBox1.Text = _mainWindow.TextBox1.Text.Insert(_mainWindow.TextBox1.CaretIndex, dateString);
                _mainWindow.TextBox1.CaretIndex = oldCaretIndex + dateString.Length;
                _datePasted = DateTime.Now;
            }

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
            ShowSetPasswordDialog(SetPasswordDialogMode.Set);
        }


        /// <summary>
        /// Returns true, if the user did positively set a password, otherwise always false;
        /// </summary>
        bool ShowSetPasswordDialog(SetPasswordDialogMode setPasswordDialogMode)
        {
            var setPassword = new SetPassword(setPasswordDialogMode, MessageBoxService)
            {
                WindowStyle = WindowStyle.ToolWindow,
                Owner = _mainWindow,
            };
            var okClicked = setPassword.ShowDialog();
            if (ModelState.Transient.FileModel.IsPasswordPresent)
                TextBlockClearPasswordVisibility = Visibility.Visible;
            else
                TextBlockClearPasswordVisibility = Visibility.Collapsed;
            RaisePropertyChanged(() => PasswordStatus);
            return okClicked == true;
        }

        #endregion

        #region FontCommand

        DelegateCommand _fontCommand;

        public DelegateCommand FontCommand
        {
            get { return CreateCommand(ref _fontCommand, ExecuteFontCommand, () => true); }
        }

        void ExecuteFontCommand()
        {
            try
            {
                ShowFontDialog();
            }
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
            }
        }



        void ShowFontDialog()
        {
            var fontDialog = new Font(_mainWindow.TextBox1.SelectedText)
             {
                 WindowStyle = WindowStyle.ToolWindow,
                 Owner = _mainWindow,
                 WindowStartupLocation = WindowStartupLocation.CenterOwner
             };

            if (fontDialog.ShowDialog() == true)
            {
                Zoom100Command.Execute();
                Map.Copy(ModelState.FontSettings, _mainWindow.TextBox1);
                ModelState.SaveSettings();
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
                var process = new Process { StartInfo = { UseShellExecute = true, FileName = Defaults.HelpUrl } };
                process.Start();
            }
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
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
            var aboutDialog = new About { WindowStyle = WindowStyle.ToolWindow, Owner = _mainWindow };
            aboutDialog.ShowDialog();
        }

        #endregion




        public void UpdateStatusBar()
        {
            var pos = UpdatePositionString();
            var enc = GetEncodingInfo();
            var contentKind = ModelState.Transient.FileModel.IsEncrypted
                ? ContentKind.EncryptedText
                : ContentKind.PlainText;
            StatusBarText = "{0} | {1} | {2}".FormatInvariant(enc, pos, contentKind);
        }
        string UpdatePositionString()
        {

            var rawPos = _mainWindow.TextBox1.CaretIndex;

            // get line and adjust for silly init values
            var lineIndex = GetCurrentLineIndex();


            // get col and adjust for silly init values
            var colIndex = GetColIndex(lineIndex);


            return "Ln {0}, Col {1} | Ch {2}/{3}".FormatInvariant(lineIndex + 1, colIndex + 1, rawPos, _mainWindow.TextBox1.Text.Length);
        }

        int GetColIndex(int lineIndex)
        {
            var rawPos = _mainWindow.TextBox1.CaretIndex;
            // get col and adjust for silly init values
            int colIndex;
            if (rawPos == 0 && lineIndex == 0)
                colIndex = 0;
            else
                colIndex = rawPos - _mainWindow.TextBox1.GetCharacterIndexFromLineIndex(lineIndex);
            return colIndex;
        }

        int GetCurrentLineIndex()
        {
            var rawPos = _mainWindow.TextBox1.CaretIndex;

            // get line and adjust for silly init values
            var lineIndex = _mainWindow.TextBox1.GetLineIndexFromCharacterIndex(rawPos);
            if (lineIndex == -1)
                lineIndex = 0;
            return lineIndex;
        }

        string GetEncodingInfo()
        {
            return ModelState.Transient.FileModel.SaveEncoding.EncodingName + ", Code Page " + ModelState.Transient.FileModel.SaveEncoding.CodePage;
        }

        void ApplySettings()
        {
            Map.Copy(ModelState.FontSettings, _mainWindow.TextBox1);
            IsStatusBarChecked = ModelState.EditorState.IsStatusBarChecked;
            IsWordWrapChecked = ModelState.EditorState.IsWordWrapChecked;
            IsSpellCheckingChecked = ModelState.EditorState.IsSpellCheckingChecked;
        }



        bool ConfirmToDiscardText()
        {
            if (ModelState.Transient.FileModel.IsDirty)
                return (MessageBoxService.Show("Discard changes?", "VisualCrypt Notepad", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
                        MessageBoxResult.OK);

            return true;
        }

        void UpdateWindowTitle()
        {
            var displayFileName = ModelState.Transient.FileModel.Filename ?? Defaults.UntitledDotVisualCrypt;
            WindowTitleText = "{0} - {1}".FormatInvariant(displayFileName, Defaults.ProductName);
        }

        void ClearEditorAndUndoStack()
        {
            _mainWindow.TextBox1.IsUndoEnabled = false;
            _mainWindow.TextBox1.Text = string.Empty;
            _mainWindow.TextBox1.IsUndoEnabled = true;
        }


        void OpenFileCommon(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
                return;

            try
            {
                var fileModel = new FileModel();
                fileModel.SetFilename(filename);
                var loadResponse = fileModel.TryLoadVisualCryptTextOrCleartext();

                if (!loadResponse.Success)
                {
                    MessageBoxService.ShowError(loadResponse.Error);
                    return;
                }

                // we succeeded loading the file, now we can replace current content with the new content.

                ModelState.Transient.FileModel = fileModel;
                ModelState.Transient.CurrentDirectoryName = Path.GetDirectoryName(filename);

                SetTextAndUpdateAllWithoutUndo(loadResponse.Result);

                if (ModelState.Transient.FileModel.IsEncrypted)
                {
                    DecryptEditorContentsCommand.Execute();
                }

            }
            catch (Exception e)
            {
                MessageBoxService.ShowError(MethodBase.GetCurrentMethod(), e);
                _mainWindow.TextBox1.Text = string.Empty;
            }

        }

        void SetTextAndUpdateAllWithoutUndo(string newText)
        {
            if (newText == null)
                throw new ArgumentNullException("newText");

            _mainWindow.TextBox1.IsUndoEnabled = false;
            _mainWindow.TextBox1.Text = newText;

            if (ModelState.Transient.FileModel.IsEncrypted)
            {
                _mainWindow.TextBox1.SpellCheck.IsEnabled = false;
                _mainWindow.TextBox1.IsUndoEnabled = false;
                _mainWindow.TextBox1.IsReadOnly = true;
            }
            else
            {
                _mainWindow.TextBox1.SpellCheck.IsEnabled = ModelState.EditorState.IsSpellCheckingChecked;
                _mainWindow.TextBox1.IsUndoEnabled = true;
                _mainWindow.TextBox1.IsReadOnly = false;

            }
            UpdateWindowTitle();
            UpdateStatusBar();

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


    }
}