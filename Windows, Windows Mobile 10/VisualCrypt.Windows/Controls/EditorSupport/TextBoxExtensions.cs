using System;
using Windows.UI.Xaml.Controls;

namespace VisualCrypt.Windows.Controls.EditorSupport
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

      
    }
}
