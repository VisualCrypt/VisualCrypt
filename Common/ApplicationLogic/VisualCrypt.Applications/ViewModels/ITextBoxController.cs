using System;

namespace VisualCrypt.Applications.ViewModels
{
    public interface ITextBoxController
    {
        object PlatformTextBox { set; }

        event EventHandler<object> TextChanged;  // textChangedEventArgs

        event EventHandler<object> SelectionChanged;  // RoutedEventArgs

        bool IsUndoEnabled { get; set; }
        string Text { get; set; }
        bool IsReadOnly { get; set; }
        double FontSize { get; set; }
        int CaretIndex { get; set; }
        int SelectionLength { get; set; }
        int LineCount { get;  }
        void SelectAll();
        void Focus();
        int GetLineLength(int currentLineIndex);
        int GetCharacterIndexFromLineIndex(int currentLineIndex);
        void Select(int indexInSourceText, int length);
        int GetLineIndexFromCharacterIndex(int rawPos);
    }

   
}