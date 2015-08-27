using System;
using VisualCrypt.Applications.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VisualCrypt.Windows.Services
{
    class TextBoxController : ITextBoxController
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
            get { return true; }
            set {; }
        }

        public string Text
        {
            get { return _textBox.Text; }
            set { _textBox.Text = value; }
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
            get { return _textBox.SelectionStart; }
            set { _textBox.SelectionStart = value; }
        }

        public int SelectionLength
        {
            get { return _textBox.SelectionLength; }
            set { _textBox.SelectionLength = value; }
        }

        public int LineCount
        {
            get
            {
                return CountViaIndexOf(_textBox.Text, Environment.NewLine);
            }
        }

        static int CountViaDifference(string source, string substring)
        {
            if (substring == "")
                return 0;
            var delta = source.Replace(substring, "");
            return (source.Length - delta.Length) / substring.Length;
        }

        static int CountViaIndexOf(string source, string substring)
        {
            int count = 0, n = 0;

            if (substring != "")
            {
                while ((n = source.IndexOf(substring, n, StringComparison.Ordinal)) != -1)
                {
                    n += substring.Length;
                    ++count;
                }
            }
            return count;
        }

        public void SelectAll()
        {
            _textBox.SelectAll();
        }

        public void Focus()
        {
            _textBox.Focus(FocusState.Programmatic);
        }

        public int GetLineLength(int currentLineIndex)
        {
            return GetLine(_textBox.Text, currentLineIndex).Length;
        }

        static string GetLine(string source, int lineIndex)
        {
            var separator = Environment.NewLine.ToCharArray();
            var lines = source.Split(separator, lineIndex + 1, StringSplitOptions.None);
            return lines[lineIndex];
        }

        // Returns the zero-based character index for the first character in the specified line.
        public int GetCharacterIndexFromLineIndex(int currentLineIndex)
        {
            return IndexOfFirstCharacterInLine(_textBox.Text, currentLineIndex);
        }

        static int  IndexOfFirstCharacterInLine(string source, int lineIndex)
        {
            if (lineIndex == 0)
                return 0;

            int count = 0, n = 0;

            while ((n = source.IndexOf(Environment.NewLine, n, StringComparison.Ordinal)) != -1)
            {
                n += Environment.NewLine.Length;
                ++count;
                if (count == lineIndex)
                    return n;
            }
            throw new Exception(nameof(CountViaIndexOf) + " is buggy");
        }

        public void Select(int indexInSourceText, int length)
        {
            _textBox.Select(indexInSourceText, length);
        }

        public int GetLineIndexFromCharacterIndex(int rawPos)
        {
            string substring = _textBox.Text.Substring(0, rawPos);
            var count = CountViaIndexOf(substring, Environment.NewLine);
            return count;
        }

        public void ApplyFontSettings(FontSettings fontSettings)
        {
            _textBox.FontFamily = fontSettings.FontFamily;
            _textBox.FontSize = fontSettings.FontSize;
            _textBox.FontStretch = fontSettings.FontStretch;
            _textBox.FontWeight = fontSettings.FontWeight;
            _textBox.FontStyle = fontSettings.FontStyle;
        }
        /// <summary>
        /// Returns a one-based line number and column of the selection start.
        /// </summary>
        /// <param name="textBox">The UWP TextBox</param>
        /// <returns>Returns a one-based line number and column of the selection start.</returns>
        /// <see cref="http://stackoverflow.com/questions/12682525/winrt-how-to-get-line-and-column-at-cursor-from-a-textbox"/>
        private static Tuple<int, int> GetPosition(TextBox textBox)
        {
            // Selection start always reports the position as though newlines are one character
            string contents = textBox.Text.Replace(Environment.NewLine, "\n");

            int i, pos = 0, line = 1;
            // Loop through all the lines up to the selection start
            while ((i = contents.IndexOf('\n', pos, textBox.SelectionStart - pos)) != -1)
            {
                pos = i + 1;
                line++;
            }

            // Column is the remaining characters
            int column = textBox.SelectionStart - pos + 1;

            return Tuple.Create(line, column);
        }
    }
}
