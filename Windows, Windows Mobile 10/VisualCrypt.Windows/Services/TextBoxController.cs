using System;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Windows.Services
{
    class TextBoxController : ITextBoxController
    {
        object _platformTextBox;
        bool _isUndoEnabled;
        string _text;
        bool _isReadOnly;
        double _fontSize;
        int _caretIndex;
        int _selectionLength;
        int _lineCount;

        public object PlatformTextBox
        {
            set { _platformTextBox = value; }
        }

        public event EventHandler<object> TextChanged;
        public event EventHandler<object> SelectionChanged;

        public bool IsUndoEnabled
        {
            get { return _isUndoEnabled; }
            set { _isUndoEnabled = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        public int CaretIndex
        {
            get { return _caretIndex; }
            set { _caretIndex = value; }
        }

        public int SelectionLength
        {
            get { return _selectionLength; }
            set { _selectionLength = value; }
        }

        public int LineCount
        {
            get { return _lineCount; }
        }

        public void SelectAll()
        {
            
        }

        public void Focus()
        {
            
        }

        public int GetLineLength(int currentLineIndex)
        {
            return 1;
        }

        public int GetCharacterIndexFromLineIndex(int currentLineIndex)
        {
            return 1;
        }

        public void Select(int indexInSourceText, int length)
        {
            
        }

        public int GetLineIndexFromCharacterIndex(int rawPos)
        {
            return 1;
        }
    }
}
