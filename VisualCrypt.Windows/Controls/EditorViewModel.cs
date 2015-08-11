using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.MVVM;
using VisualCrypt.Language;
using VisualCrypt.Windows.Controls.EditorSupport;
using VisualCrypt.Windows.Controls.EditorSupport.FindReplace;
using VisualCrypt.Windows.Events;
using VisualCrypt.Windows.Pages;
using VisualCrypt.Windows.Services;
using VisualCrypt.Windows.Static;

namespace VisualCrypt.Windows.Controls
{
    class EditorViewModel : ViewModelBase, IActiveCleanup
    {
        readonly IMessageBoxService _messageBoxService = SharedInstances.MessageBoxService;
        readonly IEventAggregator _eventAggregator = SharedInstances.EventAggregator;
        IEditor _editor;
        IEditorContext _context;

        public EditorViewModel()
        {
            _eventAggregator.GetEvent<EditorReceivesText>().Subscribe(OnTextReceived);
            _eventAggregator.GetEvent<EditorShouldSendText>().Subscribe(OnTextRequested);
            _eventAggregator.GetEvent<EditorShouldCleanup>().Subscribe(Cleanup);
        }

        public void OnViewInitialized(IEditor editor, IEditorContext context)
        {
            _editor = editor;
            _context = context;
            //SettingsManager.EditorSettings.FontSettings.ApplyTo(_editor.TextBox1);
            SearchOptions = new SearchOptions();
            //ExecuteZoom100();
            Loc.LocaleChanged += OnLocaleChanged;


            _editor.TextBox1.TextChanged += OnTextChanged;
            _editor.TextBox1.SelectionChanged += OnSelectionChanged;


            //OnTextReceived(string.Empty);
        }

        void OnLocaleChanged(object sender, EventArgs e)
        {
            //UpdateZoomLevelMenuText();
            UpdateStatusBar();
        }

        private void TextBox1_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        #region Events

        void OnTextRequested(Action<string> callback)
        {
            var args = new EditorSendsText { Text = _editor.TextBox1.Text, Callback = callback };
            _eventAggregator.GetEvent<EditorSendsText>().Publish(args);
        }


        void OnTextReceived(string newText)
        {
            if (newText == null)
                throw new ArgumentNullException("newText");

            //_editor.TextBox1.IsUndoEnabled = false;

            var backup = _context.FileModel.IsDirty;
            _editor.TextBox1.Text = newText; // triggers Text_Changed which sets FileManager.FileModel.IsDirty = true;
            _context.FileModel.IsDirty = backup;

            if (_context.FileModel.IsEncrypted)
            {
                //_editor.TextBox1.IsUndoEnabled = false;
                _editor.TextBox1.IsReadOnly = true;
            }
            else
            {
                if (_context.FileModel.SaveEncoding != null)
                {
                    //_editor.TextBox1.IsUndoEnabled = true;
                    _editor.TextBox1.IsReadOnly = false;
                }
                else
                {
                    SettingsManager.EditorSettings.IsWordWrapChecked = false;
                    //_editor.TextBox1.IsUndoEnabled = false;
                    _editor.TextBox1.IsReadOnly = true;
                }

            }
            UpdateStatusBar();
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _context.FileModel.IsDirty = true;
            UpdateStatusBar();
        }

        void OnSelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            if ((SettingsManager.EditorSettings.IsStatusBarChecked))
                UpdateStatusBar();
        }

        #endregion

        #region FindLogic

        int _timesNotFound;

        public SearchOptions SearchOptions { get; set; }

        int Pos
        {
            get { return _editor.TextBox1.CaretIndex(); }
            set { _editor.TextBox1.SetCaretIndex(value); }
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


        void ExecuteFindNextCommand()
        {
            var found = false;
            if (!SearchOptions.SearchUp)
                Pos = Pos + _editor.TextBox1.SelectionLength;

            var searchResult = Find(true, false);

            if (searchResult.HasValue)
            {
                found = true;
                SelectSearchResult(searchResult.Value.Index, searchResult.Value.Length);
            }

            if (!found && SearchOptions.UseRegEx == false)
                _messageBoxService.Show(string.Format("'{0}' is not in the document.", FindString), "Find",
                    RequestButton.OK, RequestImage.Exclamation);
            if (!found && SearchOptions.UseRegEx)
                _messageBoxService.Show(
                    string.Format("The expression '{0}' yields no matches in the document.", FindString), "Find",
                    RequestButton.OK, RequestImage.Exclamation);
        }

        SearchResult? Find(bool wrapSearchPos, bool isReplace)
        {
            _timesNotFound = 0;
            return SearchRecoursive(wrapSearchPos, isReplace);
        }

        #endregion

        #region ReplaceCommand

        public DelegateCommand ReplaceCommand
        {
            get { return CreateCommand(ref _replaceCommand, ExecuteReplaceCommand, () => !string.IsNullOrEmpty(FindString)); }
        }

        DelegateCommand _replaceCommand;


        void ExecuteReplaceCommand()
        {
            var found = false;
            if (!SearchOptions.SearchUp)
                Pos = Pos + _editor.TextBox1.SelectionLength;

            var searchResult = Find(true, true);

            if (searchResult.HasValue)
            {
                var removed = _editor.TextBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
                _editor.TextBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);

                found = true;
                SelectSearchResult(searchResult.Value.Index, ReplaceString.Length);
            }

            if (!found && SearchOptions.UseRegEx == false)
                _messageBoxService.Show(string.Format("'{0}' could not be found.", FindString), "Replace",
                    RequestButton.OK, RequestImage.Exclamation);
            if (!found && SearchOptions.UseRegEx)
                _messageBoxService.Show(string.Format("No match for '{0}' could be found.", FindString), "Replace",
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


        void ExecuteReplaceAllCommand()
        {
            SearchOptions.SearchUp = false;
            Pos = 0;
            var count = 0;

            start:
            var searchResult = Find(false, false);

            if (searchResult.HasValue)
            {
                var removed = _editor.TextBox1.Text.Remove(searchResult.Value.Index, searchResult.Value.Length);
                _editor.TextBox1.Text = removed.Insert(searchResult.Value.Index, ReplaceString);
                count++;
                Pos = searchResult.Value.Index + ReplaceString.Length;

                goto start;
            }
            var image = (count > 0) ? RequestImage.Information : RequestImage.Exclamation;


            _messageBoxService.Show(string.Format("{0} occurrences were replaced.", count), "Replace All",
                RequestButton.OK, image);
        }

        #endregion

        SearchResult? SearchRecoursive(bool wrapSearchPos, bool isReplace)
        {
            SearchResult? searchResult;
            try
            {
                searchResult = SearchStrategy.Search(_editor.TextBox1.Text, _findString, Pos, SearchOptions);
            }
            catch (ArgumentException ae)
            {
                if (SearchOptions.UseRegEx)
                {
                    _messageBoxService.Show(ae.Message, "Invalid Regular Expression Syntax", RequestButton.OK,
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
                        var result = _messageBoxService.Show(
                            "Nothing found - Search again from the top of the document?", "Replace",
                            RequestButton.OKCancel, RequestImage.Question);
                        if (result == RequestResult.Cancel || result == RequestResult.None)
                            return null;
                    }
                    Pos = 0;
                }
                else
                {
                    if (isReplace && Pos != _editor.TextBox1.Text.Length)
                    {
                        var result =
                            _messageBoxService.Show("Nothing found - Search again from the bottom of the document?",
                                "Replace",
                                RequestButton.OKCancel, RequestImage.Question);
                        if (result == RequestResult.Cancel || result == RequestResult.None)
                            return null;
                    }
                    Pos = _editor.TextBox1.Text.Length;
                }

                return SearchRecoursive(true, isReplace);
            }
            _timesNotFound = 0;
            return null;
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
                if (_editor != null)
                    return _editor.TextBox1.LineCount();
                return 0;
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
            if (_editor.TextBox1.LineCount() < lineNo)
                return false;
            _lineIndex = lineNo - 1;
            return true;
        }

        void ExecuteGoButtonCommand()
        {
            try
            {
                var index = _editor.TextBox1.GetCharacterIndexFromLineIndex(_lineIndex);
                _editor.TextBox1.SetCaretIndex(index);
                var lineLength = _editor.TextBox1.GetLineLength(_lineIndex);
                SelectSearchResult(index, lineLength);
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        void SelectSearchResult(int indexInSourceText, int length)
        {
            _editor.TextBox1.Select(indexInSourceText, length);
            _editor.TextBox1.Focus();
        }

        #endregion

        #region Private methods

        internal void UpdateStatusBar()
        {
            OnPropertyChanged(() => LineCount);

            var pos = GetPositionString();
            var enc = GetEncodingString();

            var statusBarText = string.Format(CultureInfo.InvariantCulture, Loc.Strings.plaintextStatusbarText, pos, enc);
            _eventAggregator.GetEvent<EditorSendsStatusBarInfo>().Publish(statusBarText);
        }

        string GetEncodingString()
        {
            return _context.FileModel.SaveEncoding != null ? _context.FileModel.SaveEncoding.EncodingName : "Hex View";
        }

        string GetPositionString()
        {
            var rawPos = _editor.TextBox1.CaretIndex();
            var lineIndex = GetCurrentLineIndex();
            var colIndex = GetColIndex(lineIndex);

            return string.Format(CultureInfo.InvariantCulture, Loc.Strings.plaintextStatusbarPositionInfo, lineIndex + 1, colIndex + 1, rawPos,
                _editor.TextBox1.Text.Length);
        }

        int GetColIndex(int lineIndex)
        {
            var rawPos = _editor.TextBox1.CaretIndex();
            // get col and adjust for silly init values
            int colIndex;
            if (rawPos == 0 && lineIndex == 0)
                colIndex = 0;
            else
                colIndex = rawPos - _editor.TextBox1.GetCharacterIndexFromLineIndex(lineIndex);
            return colIndex;
        }

        int GetCurrentLineIndex()
        {
            var rawPos = _editor.TextBox1.CaretIndex();

            // get line and adjust for silly init values
            var lineIndex = _editor.TextBox1.GetLineIndexFromCharacterIndex(rawPos);
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
            _editor.TextBox1.TextChanged -= OnTextChanged;
            _editor.TextBox1.SelectionChanged -= OnSelectionChanged;
        }

        #endregion
    }
}
