using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Desktop.ModuleEditor.Features.Printing;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.Events;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.Services;
using VisualCrypt.Desktop.Shared.Settings;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
    [Export]
    public class EditorViewModel : ViewModelBase
    {
        readonly Window _mainWindow = Application.Current.MainWindow;
        TextBox _textBox1;
        SpellCheck _spellCheck;
        private FindReplaceViewModel _findReplaceDialogViewModel;
        GoToViewModel _goToWindowViewModel;
        readonly IMessageBoxService _messageBoxService = new MessageBoxService(Application.Current.MainWindow);
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public EditorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void OnEditorInitialized(TextBox textbox1)
        {
            _textBox1 = textbox1;

            _spellCheck = _textBox1.SpellCheck;
            _spellCheck.SpellingReform = SpellingReform.Postreform;

            _findReplaceDialogViewModel = new FindReplaceViewModel(_textBox1);
            _goToWindowViewModel = new GoToViewModel(_textBox1);

            ApplyEditorSettings();
            ExecuteZoom100();
            OnTextReceived(string.Empty);

            _eventAggregator.GetEvent<EditorReceivesText>().Subscribe(OnTextReceived);
            _eventAggregator.GetEvent<EditorShouldSendText>().Subscribe(OnShouldSendText);

        }

        void OnShouldSendText(Action<string> callback)
        {
            var args = new EditorSendsText { Text = _textBox1.Text, Callback = callback };
            _eventAggregator.GetEvent<EditorSendsText>().Publish(args);
        }

        void ApplyEditorSettings()
        {
            if (SettingsManager.EditorSettings.FontSettings.FontFamily == null)
                SettingsManager.EditorSettings.FontSettings.FontFamily = SystemFonts.MessageFontFamily;
            if (SettingsManager.EditorSettings.FontSettings.FontSize < 5)
                SettingsManager.EditorSettings.FontSettings.FontSize = SystemFonts.MessageFontSize;

            Map.Copy(SettingsManager.EditorSettings.FontSettings, _textBox1);

          
        }




        #region TextChangedCommand

        public void ExecuteTextChangedCommand(TextChangedEventArgs e)
        {
            FileManager.FileModel.IsDirty = true;
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

        public void ExecuteSelectionChangedCommand(RoutedEventArgs args)
        {
            if ((SettingsManager.EditorSettings.IsStatusBarChecked))
                UpdateStatusBar();
        }

        #endregion


        #region ZoomCommands



        public bool CanExecuteZoomIn()
        {
            return _textBox1.FontSize < 999;
        }

        public void ExecuteZoomIn()
        {
            _textBox1.FontSize *= 1.05;
            UpdateZoomLevelMenuText();

            //Zoom100Command.RaiseCanExecuteChanged();
            //ZoomInCommand.RaiseCanExecuteChanged();
            //ZoomOutCommand.RaiseCanExecuteChanged();
        }



        public bool CanExecuteZoomOut()
        {
            return _textBox1.FontSize > 1;
        }

        public void ExecuteZoomOut()
        {
            _textBox1.FontSize *= 1 / 1.05;
            UpdateZoomLevelMenuText();

            //Zoom100Command.RaiseCanExecuteChanged();
            //ZoomInCommand.RaiseCanExecuteChanged();
            //ZoomOutCommand.RaiseCanExecuteChanged();
        }



        public bool CanExecuteZoom100()
        {
            return !SettingsManager.EditorSettings.IsZoom100Checked;
        }
        public void ExecuteZoom100()
        {
            _textBox1.FontSize = SettingsManager.EditorSettings.FontSettings.FontSize;
            UpdateZoomLevelMenuText();

            //Zoom100Command.RaiseCanExecuteChanged();
            //ZoomInCommand.RaiseCanExecuteChanged();
            //ZoomOutCommand.RaiseCanExecuteChanged();
        }

        void UpdateZoomLevelMenuText()
        {
            var zoomLevel = (int)((_textBox1.FontSize / SettingsManager.EditorSettings.FontSettings.FontSize) * 100);
            var zoomLevelMenuText = "_Zoom (" + zoomLevel + "%)";
            SettingsManager.EditorSettings.ZoomLevelMenuText = zoomLevelMenuText;

            SettingsManager.EditorSettings.IsZoom100Checked = Math.Abs(((_textBox1.FontSize / SettingsManager.EditorSettings.FontSettings.FontSize) * 100) - 100) < 0.1;
        }


        #endregion

        #region PrintCommand

        public bool CanExecutePrint()
        {
            return true;
        }

        public void ExecutePrint()
        {
            try
            {
                var doc = Printer.ConvertToFlowDocument(_textBox1.Text);
                Printer.PrintFlowDocument(doc, SettingsManager.EditorSettings.PrintMargin);
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }


        #endregion

        #region FindCommand

        public bool CanExecuteFind()
        {
            return _textBox1.Text.Length > 0;
        }

        public void ExecuteFind()
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
            findReplaceDialog.Closed += (o, args) => _textBox1.Focus();
            _findReplaceDialogViewModel.MessageBoxService = new MessageBoxService(findReplaceDialog);

        }

        #endregion

        #region FindNextCommand





        public void ExecuteFindNext()
        {

            var oldDirection = _findReplaceDialogViewModel.SearchOptions.SearchUp;
            _findReplaceDialogViewModel.SearchOptions.SearchUp = false;
            _findReplaceDialogViewModel.FindNextCommand.Execute();
            _findReplaceDialogViewModel.SearchOptions.SearchUp = oldDirection;
        }

        public bool CanExecuteFindNext()
        {
            return _findReplaceDialogViewModel.FindNextCommand.CanExecute() &&
                   !string.IsNullOrEmpty(_findReplaceDialogViewModel.FindString);
        }


        #endregion

        #region FindPreviousCommand





        public void ExecuteFindPrevious()
        {
            var oldDirection = _findReplaceDialogViewModel.SearchOptions.SearchUp;
            _findReplaceDialogViewModel.SearchOptions.SearchUp = true;
            _findReplaceDialogViewModel.FindNextCommand.Execute();
            _findReplaceDialogViewModel.SearchOptions.SearchUp = oldDirection;
        }

        public bool CanExecuteFindPrevious()
        {
            return _findReplaceDialogViewModel.FindNextCommand.CanExecute() &&
                   !string.IsNullOrEmpty(_findReplaceDialogViewModel.FindString);
        }


        #endregion

        #region ReplaceCommand

        public bool CanExecuteReplace()
        {
            return _textBox1.Text.Length > 0;
        }


        public void ExecuteReplace()
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

        public bool CanExecuteDeleteLine()
        {
            return true;
        }

        public void ExecuteDeleteLine()
        {
            try
            {
                var currentLineIndex = GetCurrentLineIndex();

                // 1.) Get col in line to delete
                var currentColIndex = GetColIndex(currentLineIndex);

                // 2.) Is there a next line?
                var isNextLine = _textBox1.LineCount > currentLineIndex + 1;

                var currentLineLenght = _textBox1.GetLineLength(currentLineIndex);
                if (currentLineLenght < 1)
                    return;


                var targetColIndex = 0;
                if (isNextLine)
                {
                    var nextLineLenght = _textBox1.GetLineLength(currentLineIndex + 1);
                    var nextLineIsAtLeastAsLongAsCurrentLine = nextLineLenght >= currentLineLenght;

                    if (nextLineIsAtLeastAsLongAsCurrentLine)
                    {
                        targetColIndex = currentColIndex;
                    }
                    else
                    {
                        targetColIndex = _textBox1.GetLineLength(currentLineIndex + 1) - 2;
                        if (targetColIndex < 0)
                            targetColIndex = 0;
                    }
                }
                // targetColIndex stays 0 if there is no next line!


                var firstPos = _textBox1.GetCharacterIndexFromLineIndex(currentLineIndex);
                _textBox1.Text = _textBox1.Text.Remove(firstPos, currentLineLenght);



                var newPos = _textBox1.GetCharacterIndexFromLineIndex(currentLineIndex) + targetColIndex;
                _textBox1.CaretIndex = newPos;
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }

        }



        #endregion

        #region GoToCommand

        public void ExecuteGoTo()
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

        public bool CanExecuteGoTo()
        {
            return _textBox1.LineCount > 1;

        }

        #endregion

        #region DateCommand


        private DateTime? _datePasted;
        public bool CanExecuteInsertDateTime()
        {
            return true;
        }

        public void ExecuteInsertDateTime()
        {
            if (_datePasted.HasValue && DateTime.Now - _datePasted.Value < TimeSpan.FromSeconds(3))
            {
                var oldCaretIndex = _textBox1.CaretIndex;
                var timeStringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
                var timeString = " " + DateTime.Now.ToString(timeStringPattern);
                _textBox1.Text = _textBox1.Text.Insert(_textBox1.CaretIndex, timeString);
                _textBox1.CaretIndex = oldCaretIndex + 1 + timeString.Length;
                _datePasted = null;
            }
            else
            {
                var oldCaretIndex = _textBox1.CaretIndex;
                var datetringPattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                var dateString = DateTime.Now.ToString(datetringPattern);
                _textBox1.Text = _textBox1.Text.Insert(_textBox1.CaretIndex, dateString);
                _textBox1.CaretIndex = oldCaretIndex + dateString.Length;
                _datePasted = DateTime.Now;
            }
        }

        #endregion

        #region FontCommand

        public bool CanExecuteFont()
        {
            return true;
        }

        public void ExecuteFont()
        {
            try
            {
                ShowFontDialog();
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }



        void ShowFontDialog()
        {
            var fontDialog = new Font(_textBox1.SelectedText)
            {
                WindowStyle = WindowStyle.ToolWindow,
                Owner = _mainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (fontDialog.ShowDialog() == true)
            {
                ExecuteZoom100();
                Map.Copy(SettingsManager.EditorSettings.FontSettings, _textBox1);
                SettingsManager.SaveSettings();
            }

        }

        #endregion

        async void OnTextReceived(string newText)
        {
            if (newText == null)
                throw new ArgumentNullException("newText");

            

            _textBox1.IsUndoEnabled = false;
            _textBox1.Text = newText;
            FileManager.FileModel.IsDirty = false;

            if (FileManager.FileModel.IsEncrypted)
            {
                _textBox1.SpellCheck.IsEnabled = false;
                _textBox1.IsUndoEnabled = false;
                _textBox1.IsReadOnly = true;
            }
            else
            {
                _textBox1.SpellCheck.IsEnabled = SettingsManager.EditorSettings.IsSpellCheckingChecked;
                _textBox1.IsUndoEnabled = true;
                _textBox1.IsReadOnly = false;

            }

            UpdateStatusBar();

        }

        #region private methods

       

        void UpdateStatusBar()
        {
            var pos = UpdatePositionString();
            var enc = GetEncodingInfo();

            var statusBarText = "{0} | {1} | Encrypted: {2}".FormatInvariant(enc, pos, FileManager.FileModel.IsEncrypted);
            _eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Publish(statusBarText);
        }

        string GetEncodingInfo()
        {
           return FileManager.FileModel.SaveEncoding.EncodingName + ", Code Page " + FileManager.FileModel.SaveEncoding.CodePage;
        }
        string UpdatePositionString()
        {

            var rawPos = _textBox1.CaretIndex;

            // get line and adjust for silly init values
            var lineIndex = GetCurrentLineIndex();


            // get col and adjust for silly init values
            var colIndex = GetColIndex(lineIndex);


            return "Ln {0}, Col {1} | Ch {2}/{3}".FormatInvariant(lineIndex + 1, colIndex + 1, rawPos, _textBox1.Text.Length);
        }

        int GetColIndex(int lineIndex)
        {
            var rawPos = _textBox1.CaretIndex;
            // get col and adjust for silly init values
            int colIndex;
            if (rawPos == 0 && lineIndex == 0)
                colIndex = 0;
            else
                colIndex = rawPos - _textBox1.GetCharacterIndexFromLineIndex(lineIndex);
            return colIndex;
        }

        int GetCurrentLineIndex()
        {
            var rawPos = _textBox1.CaretIndex;

            // get line and adjust for silly init values
            var lineIndex = _textBox1.GetLineIndexFromCharacterIndex(rawPos);
            if (lineIndex == -1)
                lineIndex = 0;
            return lineIndex;
        }

        #endregion



    }
}
