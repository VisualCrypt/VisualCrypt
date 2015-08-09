using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace VisualCrypt.Windows.Controls
{
    public static class TextBoxExtensions
    {
        /// <summary>
        /// Equivalent for textBox.CaretIndex.
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static int CaretIndex(this TextBox textBox)
        {
            return textBox.SelectionStart;
        }

        public static void  SetCaretIndex(this TextBox textBox, int value)
        {
           textBox.SelectionStart = value;
        }

        /// <summary>
        /// Equivalent for textBox.GetCharacterIndexFromLineIndex(lineIndex).
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static int GetCharacterIndexFromLineIndex(this TextBox textBox, int lineIndex)
        {
            return lineIndex * 10;
        }
        //GetLineIndexFromCharacterIndex
        public static int GetLineIndexFromCharacterIndex(this TextBox textBox, int characterIndex)
        {
            return  5;
        }

        public static int GetLineLength(this TextBox textBox, int lineIndex)
        {
            return lineIndex * 10;
        }

        public static void Focus(this TextBox textBox)
        {
            
        }

        public static int LineCount(this TextBox textBox)
        {
            return 20;
        }

        // http://stackoverflow.com/questions/12682525/winrt-how-to-get-line-and-column-at-cursor-from-a-textbox
        // Returns a one-based line number and column of the selection start
        private static Tuple<int, int> GetPosition(TextBox text)
        {
            // Selection start always reports the position as though newlines are one character
            string contents = text.Text.Replace(Environment.NewLine, "\n");

            int i, pos = 0, line = 1;
            // Loop through all the lines up to the selection start
            while ((i = contents.IndexOf('\n', pos, text.SelectionStart - pos)) != -1)
            {
                pos = i + 1;
                line++;
            }

            // Column is the remaining characters
            int column = text.SelectionStart - pos + 1;

            return Tuple.Create(line, column);
        }
    }
}
