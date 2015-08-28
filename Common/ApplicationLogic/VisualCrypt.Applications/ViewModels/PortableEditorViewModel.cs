using System;
using System.Globalization;
using Prism.Commands;
using Prism.Events;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Events;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels.FindReplace;
using VisualCrypt.Language;
using System.Threading.Tasks;

namespace VisualCrypt.Applications.ViewModels
{

    public class PortableEditorViewModel : ViewModelBase
    {
        #region Initialization


        readonly IMessageBoxService _messageBoxService;
        readonly IEventAggregator _eventAggregator;
        readonly IWindowManager _windowManager;
        readonly ISettingsManager _settingsManager;
        readonly IEditorContext _editorContext;
        readonly IPrinter _printer;
        readonly IFontManager _fontManager;
        readonly ITextBoxController _textBox1;
        readonly ITextBoxController _textBoxFind;
        readonly ITextBoxController _textBoxFindReplace;
        readonly ITextBoxController _textBoxGoTo;


        public PortableEditorViewModel()
        {
            _eventAggregator = Service.Get<IEventAggregator>();
            _messageBoxService = Service.Get<IMessageBoxService>();
            _windowManager = Service.Get<IWindowManager>();
            _settingsManager = Service.Get<ISettingsManager>();
            _editorContext = Service.Get<PortableMainViewModel>();
            _printer = Service.Get<IPrinter>();
            _fontManager = Service.Get<IFontManager>();
            _textBox1 = Service.Get<ITextBoxController>(TextBoxName.TextBox1);
            _textBoxFind = Service.Get<ITextBoxController>(TextBoxName.TextBoxFind);
            _textBoxFindReplace = Service.Get<ITextBoxController>(TextBoxName.TextBoxFindReplace);
            _textBoxGoTo = Service.Get<ITextBoxController>(TextBoxName.TextBoxGoTo);
        }

        public void OnViewLoaded()
        {
            _fontManager.ApplyFontsFromSettingsToEditor();

            SearchOptions = new SearchOptions();
            _fontManager.ExecuteZoom100();
            Loc.LocaleChanged += OnLocaleChanged;


            _textBox1.TextChanged += OnTextChanged;
            _textBox1.SelectionChanged += OnSelectionChanged;
            _eventAggregator.GetEvent<EditorReceivesText>().Subscribe(OnTextReceived);
            _eventAggregator.GetEvent<EditorShouldSendText>().Subscribe(OnTextRequested);
            _eventAggregator.GetEvent<EditorShouldCleanup>().Subscribe(Cleanup);

            OnTextReceived(string.Empty);
        }

        void OnLocaleChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        #endregion

        #region Bound Properties

        public ISettingsManager SettingsManager
        {
            get { return _settingsManager; }
        }

        #endregion

        #region Events

        void OnTextRequested(Action<string> callback)
        {
            var args = new EditorSendsText { Text = _textBox1.Text, Callback = callback };
            _eventAggregator.GetEvent<EditorSendsText>().Publish(args);
        }


        void OnTextReceived(string newText)
        {
            if (newText == null)
                throw new ArgumentNullException("newText");

            _textBox1.IsUndoEnabled = false;

            var backup = _editorContext.FileModel.IsDirty;
            _textBox1.Text = newText; // triggers Text_Changed which sets FileService.FileModel.IsDirty = true;
            _editorContext.FileModel.IsDirty = backup;

            if (_editorContext.FileModel.IsEncrypted)
            {
                _textBox1.IsUndoEnabled = false;
                _textBox1.IsReadOnly = true;
            }
            else
            {
                if (_editorContext.FileModel.SaveEncoding != null)
                {
                    _textBox1.IsUndoEnabled = true;
                    _textBox1.IsReadOnly = false;
                }
                else
                {
                    _settingsManager.EditorSettings.IsWordWrapChecked = false;
                    _textBox1.IsUndoEnabled = false;
                    _textBox1.IsReadOnly = true;
                }

            }
            UpdateStatusBar();
        }

        void OnTextChanged(object sender, object textChangedEventArgs)
        {
            _editorContext.FileModel.IsDirty = true;
            UpdateStatusBar();
        }

        void OnSelectionChanged(object sender, object routedEventArgs)
        {
            if ((_settingsManager.EditorSettings.IsStatusBarVisible))
                UpdateStatusBar();
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
                _printer.PrintEditorText(_textBox1.Text);
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        #endregion

        #region CloseToolAreaCommand

        public int ToolAreaSelectedIndex
        {
            get { return _toolAreaSelectedIndex; }
            set
            {
                if (_toolAreaSelectedIndex != value)
                {
                    _toolAreaSelectedIndex = value;
                    OnPropertyChanged(() => ToolAreaSelectedIndex);
                }
            }
        }

        int _toolAreaSelectedIndex;

        public DelegateCommand CloseToolAreaCommand
        {
            get
            {
                return CreateCommand(ref _closeToolAreaCommand, () => _settingsManager.EditorSettings.IsToolAreaVisible = false,
                    () => true);
            }
        }
        DelegateCommand _closeToolAreaCommand;
        #endregion

        #region FindMenuCommand

        public bool CanExecuteFindMenuCommand()
        {
            return _textBox1.Text.Length > 0;
        }

        public void ExecuteFindMenuCommand()
        {
            try
            {
                _settingsManager.EditorSettings.IsToolAreaVisible = true;
                ToolAreaSelectedIndex = 0;
                //_portableEditorView.EditorControl.UpdateLayout();
                _textBoxFind.SelectAll();
                _textBoxFind.Focus();
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        #endregion

        #region FindNextMenuCommand

        public async void ExecuteFindNextMenuCommand()
        {
            var oldDirection = SearchOptions.SearchUp;
            SearchOptions.SearchUp = false;
            await FindNextButtonCommand.Execute();
            SearchOptions.SearchUp = oldDirection;
        }

        public bool CanExecuteFindNextMenuCommand()
        {
            return FindNextButtonCommand.CanExecute() &&
                   !string.IsNullOrEmpty(FindString);
        }

        #endregion

        #region FindPreviousMenuCommand

        public async void ExecuteFindPreviousMenuCommand()
        {
            var oldDirection = SearchOptions.SearchUp;
            SearchOptions.SearchUp = true;
            await FindNextButtonCommand.Execute();
            SearchOptions.SearchUp = oldDirection;
        }

        public bool CanExecuteFindPreviousMenuCommand()
        {
            return FindNextButtonCommand.CanExecute() &&
                   !string.IsNullOrEmpty(FindString);
        }

        #endregion

        #region ReplaceMenuCommand

        public bool CanExecuteReplaceMenuCommand()
        {
            return _textBox1.Text.Length > 0 && _editorContext.FileModel.SaveEncoding != null;
        }

        public void ExecuteReplaceMenuCommand()
        {
            _settingsManager.EditorSettings.IsToolAreaVisible = true;
            ToolAreaSelectedIndex = 1;
            //_portableEditorView.EditorControl.UpdateLayout();
            _textBoxFindReplace.SelectAll();
            _textBoxFindReplace.Focus();
        }

        #endregion

        #region FindLogic

        int _timesNotFound;

        public SearchOptions SearchOptions { get; set; }

        int Pos
        {
            get { return _textBox1.CaretIndex; }
            set { _textBox1.CaretIndex = value; }
        }


        public string FindString
        {
            get { return _findString; }
            set
            {
                if (_findString == value) return;
                _findString = value;
                OnPropertyChanged(() => FindString);

                FindNextButtonCommand.RaiseCanExecuteChanged();
                ReplaceCommand.RaiseCanExecuteChanged();
                ReplaceAllCommand.RaiseCanExecuteChanged();
            }
        }
        string _findString = string.Empty;

        public string ReplaceString
        {
            get { return _replaceString; }
            set
            {
                if (_replaceString == value) return;
                _replaceString = value;
                OnPropertyChanged(() => ReplaceString);
            }
        }

        string _replaceString = string.Empty;

        #endregion

        #region FindNextButtonCommand

        public DelegateCommand FindNextButtonCommand
        {
            get
            {
                return CreateCommand(ref _findNextCommand, ExecuteFindNextCommand,
                    () => !string.IsNullOrEmpty(FindString));
            }
        }

        DelegateCommand _findNextCommand;


        async void ExecuteFindNextCommand()
        {
            var found = false;
            if (!SearchOptions.SearchUp)
                Pos = Pos + _textBox1.SelectionLength;

            var searchResult = await Find(true, false);

            if (searchResult.HasValue)
            {
                found = true;
                SelectSearchResult(searchResult.Value.Index, searchResult.Value.Length);
            }

            if (!found && SearchOptions.UseRegEx == false)
                await _messageBoxService.Show(string.Format("'{0}' is not in the document.", FindString), "Find",
                    RequestButton.OK, RequestImage.Exclamation);
            if (!found && SearchOptions.UseRegEx)
                await _messageBoxService.Show(
                    string.Format("The expression '{0}' yields no matches in the document.", FindString), "Find",
                    RequestButton.OK, RequestImage.Exclamation);
        }

        async Task<SearchResult?> Find(bool wrapSearchPos, bool isReplace)
        {
            _timesNotFound = 0;
            return await SearchRecoursive(wrapSearchPos, isReplace);
        }

        #endregion

        #region ReplaceCommand

        public DelegateCommand ReplaceCommand
        {
            get { return CreateCommand(ref _replaceCommand, ExecuteReplaceCommand, () => !string.IsNullOrEmpty(FindString)); }
        }

        DelegateCommand _replaceCommand;

        async void ExecuteReplaceCommand()
        {
            var found = false;
            if (!SearchOptions.SearchUp)
                Pos = Pos + _textBox1.SelectionLength;

            var searchResult = await Find(true, true);

            if (searchResult.HasValue)
            {
                var removed = _textBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
                _textBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);

                found = true;
                SelectSearchResult(searchResult.Value.Index, ReplaceString.Length);
            }

            if (!found && SearchOptions.UseRegEx == false)
                await _messageBoxService.Show(string.Format("'{0}' could not be found.", FindString), "Replace",
                    RequestButton.OK, RequestImage.Exclamation);
            if (!found && SearchOptions.UseRegEx)
                await _messageBoxService.Show(string.Format("No match for '{0}' could be found.", FindString), "Replace",
                    RequestButton.OK, RequestImage.Exclamation);
        }

        #endregion

        #region ReplaceAllCommand

        public DelegateCommand ReplaceAllCommand
        {
            get
            {
                return CreateCommand(ref _replaceAllCommand, ExecuteReplaceAllCommand,
                    () => !string.IsNullOrEmpty(FindString));
            }
        }

        DelegateCommand _replaceAllCommand;


        async void ExecuteReplaceAllCommand()
        {
            SearchOptions.SearchUp = false;
            Pos = 0;
            var count = 0;

            start:
            var searchResult = await Find(false, false);

            if (searchResult.HasValue)
            {
                var removed = _textBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
                _textBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);
                count++;
                Pos = searchResult.Value.Index + ReplaceString.Length;

                goto start;
            }
            var image = (count > 0) ? RequestImage.Information : RequestImage.Exclamation;


            await _messageBoxService.Show(string.Format("{0} occurrences were replaced.", count), "Replace All",
                RequestButton.OK, image);
        }

        async System.Threading.Tasks.Task<SearchResult?> SearchRecoursive(bool wrapSearchPos, bool isReplace)
        {
            SearchResult? searchResult;
            try
            {
                searchResult = SearchStrategy.Search(_textBox1.Text, _findString, Pos, SearchOptions);
            }
            catch (ArgumentException ae)
            {
                if (SearchOptions.UseRegEx)
                {
                    await _messageBoxService.Show(ae.Message, "Invalid Regular Expression Syntax", RequestButton.OK,
                        RequestImage.Error);
                    return null;
                }
                throw;
            }
            if (searchResult.HasValue)
            {
                return searchResult;
            }
            // if not found..
            if (!wrapSearchPos)
                return null;

            _timesNotFound += 1;
            if (_timesNotFound < 2)
            {
                if (!SearchOptions.SearchUp)
                {
                    if (isReplace && Pos != 0)
                    {
                        var result = await _messageBoxService.Show(
                            "Nothing found - Search again from the top of the document?", "Replace",
                            RequestButton.OKCancel, RequestImage.Question);
                        if (result == RequestResult.Cancel || result == RequestResult.None)
                            return null;
                    }
                    Pos = 0;
                }
                else
                {
                    if (isReplace && Pos != _textBox1.Text.Length)
                    {
                        var result = await
                            _messageBoxService.Show("Nothing found - Search again from the bottom of the document?",
                                "Replace",
                                RequestButton.OKCancel, RequestImage.Question);
                        if (result == RequestResult.Cancel || result == RequestResult.None)
                            return null;
                    }
                    Pos = _textBox1.Text.Length;
                }

                return await SearchRecoursive(true, isReplace);
            }
            _timesNotFound = 0;
            return null;
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

                var currentLineLength = _textBox1.GetLineLength(currentLineIndex);
                if (currentLineLength < 1)
                    return;


                var targetColIndex = 0;
                if (isNextLine)
                {
                    var nextLineLength = _textBox1.GetLineLength(currentLineIndex + 1);
                    var nextLineIsAtLeastAsLongAsCurrentLine = nextLineLength >= currentLineLength;

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
                _textBox1.Text = _textBox1.Text.Remove(firstPos, currentLineLength);


                var newPos = _textBox1.GetCharacterIndexFromLineIndex(currentLineIndex) + targetColIndex;
                _textBox1.CaretIndex = newPos;
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        #endregion

        #region GoButtonCommand

        public string LineNo
        {
            get { return _lineNo; }
            set
            {
                if (_lineNo == value) return;
                _lineNo = value;
                OnPropertyChanged(() => LineNo);

                GoButtonCommand.RaiseCanExecuteChanged();
            }
        }

        string _lineNo;

        public int LineCount
        {
            get
            {
                return _textBox1.LineCount;
            }
        }


        public DelegateCommand GoButtonCommand
        {
            get { return CreateCommand(ref _goButtonCommand, ExecuteGoButtonCommand, CanExecuteGoButtonCommand); }
        }

        DelegateCommand _goButtonCommand;

        int _lineIndex;

        bool CanExecuteGoButtonCommand()
        {
            OnPropertyChanged(() => LineCount);
            int lineNo;
            var canParse = int.TryParse(LineNo, out lineNo);
            if (!canParse)
                return false;
            if (lineNo <= 0)
                return false;
            if (_textBox1.LineCount < lineNo)
                return false;
            _lineIndex = lineNo - 1;
            return true;
        }

        void ExecuteGoButtonCommand()
        {
            try
            {
                var index = _textBox1.GetCharacterIndexFromLineIndex(_lineIndex);
                _textBox1.CaretIndex = index;
                var lineLength = _textBox1.GetLineLength(_lineIndex);
                SelectSearchResult(index, lineLength);
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        void SelectSearchResult(int indexInSourceText, int length)
        {
            _textBox1.Select(indexInSourceText, length);
            _textBox1.Focus();
        }

        #endregion

        #region GoMenuCommand

        public void ExecuteGoMenuCommand()
        {
            try
            {
                _settingsManager.EditorSettings.IsToolAreaVisible = true;
                ToolAreaSelectedIndex = 2;
                //_portableEditorView.EditorControl.UpdateLayout();
                _textBoxGoTo.SelectAll();
                _textBoxGoTo.Focus();
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        public bool CanExecuteGoMenuCommand()
        {
            return _textBox1.LineCount > 0;
        }

        #endregion

        #region DateCommand

        DateTime? _datePasted;

        public bool CanExecuteInsertDateTime()
        {
            return _editorContext.FileModel.SaveEncoding != null;
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


        #region Private methods

        public void UpdateStatusBar()
        {
            OnPropertyChanged(() => LineCount);

            var pos = GetPositionString();
            var enc = GetEncodingString();

            var statusBarText = string.Format(CultureInfo.InvariantCulture, Loc.Strings.plaintextStatusbarText, pos, enc);
            _eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Publish(statusBarText);
        }

        string GetEncodingString()
        {
            return _editorContext.FileModel.SaveEncoding != null ?
                _editorContext.FileModel.SaveEncoding.ToString()  // This was SaveEncoding.EncodingName
                : "Hex View";
        }

        string GetPositionString()
        {
            var rawPos = _textBox1.CaretIndex;
            var lineIndex = GetCurrentLineIndex();
            var colIndex = GetColIndex(lineIndex);

            return string.Format(CultureInfo.InvariantCulture, Loc.Strings.plaintextStatusbarPositionInfo, lineIndex + 1, colIndex + 1, rawPos,
                _textBox1.Text.Length);
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

        #region IActiveCleanup

        void Cleanup(EditorShouldCleanup args)
        {
            Cleanup();
        }

        public void Cleanup()
        {
            _eventAggregator.GetEvent<EditorReceivesText>().Unsubscribe(OnTextReceived);
            _eventAggregator.GetEvent<EditorShouldSendText>().Unsubscribe(OnTextRequested);
            _eventAggregator.GetEvent<EditorShouldCleanup>().Unsubscribe(Cleanup);
            Loc.LocaleChanged -= OnLocaleChanged;
            _textBox1.TextChanged -= OnTextChanged;
            _textBox1.SelectionChanged -= OnSelectionChanged;
        }

        #endregion
    }
}