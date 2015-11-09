using System;
using System.Windows;
using System.Windows.Controls;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Desktop.Settings;

namespace VisualCrypt.Desktop.Services
{
    sealed class TextBoxController : ITextBoxController
    {
        public event EventHandler<object> TextChanged;
        public event EventHandler<object> SelectionChanged;

        TextBox _textBox;

        public object PlatformTextBox
        {
            set
            {
                _textBox = value as TextBox;
                if (_textBox == null)
                    return;
                _textBox.TextChanged += OnTextChanged;
                _textBox.SelectionChanged += OnSelectionChnanged;
            }
        }

        void OnSelectionChnanged(object sender, RoutedEventArgs e)
        {
            var handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var handler = TextChanged;
            handler?.Invoke(this, e);
        }

        public bool IsUndoEnabled
        {
            get { return _textBox.IsUndoEnabled; }
            set { _textBox.IsUndoEnabled = value; }
        }

        public string Text
        {
            get { return _textBox.Text; }
            set { _textBox.Text = value;
            }
        }

        public bool IsReadOnly
        {
            get { return _textBox.IsReadOnly; }
            set { _textBox.IsReadOnly = value; }
        }

        public double FontSize
        {
            get { return _textBox.FontSize; }
            set { _textBox.FontSize = value; }
        }

        public int CaretIndex
        {
            get { return _textBox.CaretIndex; }
            set { _textBox.CaretIndex = value; }
        }

        public int SelectionLength
        {
            get { return _textBox.SelectionLength; }
            set { _textBox.SelectionLength = value; }
        }

        public int LineCount
        {
            get { return _textBox.LineCount; }
        }

        public void SelectAll()
        {
            _textBox.SelectAll();
        }

        public void Focus()
        {
            _textBox.Focus();
        }

        public int GetLineLength(int currentLineIndex)
        {
            return _textBox.GetLineLength(currentLineIndex);
        }

        public int GetCharacterIndexFromLineIndex(int currentLineIndex)
        {
            return _textBox.GetCharacterIndexFromLineIndex(currentLineIndex);
        }

        public void Select(int indexInSourceText, int length)
        {
            _textBox.Select(indexInSourceText, length);
        }

        public int GetLineIndexFromCharacterIndex(int rawPos)
        {
            return _textBox.GetLineIndexFromCharacterIndex(rawPos);
        }

        public void ApplyFontSettings(FontSettings fontSettings)
        {
            _textBox.FontFamily = fontSettings.FontFamily;
            _textBox.FontSize = fontSettings.FontSize;
            _textBox.FontStretch = fontSettings.FontStretch;
            _textBox.FontWeight = fontSettings.FontWeight;
            _textBox.FontStyle = fontSettings.FontStyle;
        }
    }
}
