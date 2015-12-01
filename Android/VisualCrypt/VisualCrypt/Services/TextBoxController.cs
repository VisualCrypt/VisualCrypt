using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Droid.Services
{
    class TextBoxController : ITextBoxController
    {
        EditText _textBox;
        public object PlatformTextBox
        {
            set
            {
                _textBox = value as EditText;
                if (_textBox == null)
                    return;
                _textBox.TextChanged += OnTextChanged;

            }
        }

        void OnTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {

            var handler = TextChanged;
            handler?.Invoke(this, e);
        }

        public int CaretIndex
        {
            get
            {
                return _textBox.SelectionStart;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public double FontSize
        {
            get
            {
                return 10;
            }

            set
            {
                ;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }

            set
            {
                ;
            }
        }

        public bool IsUndoEnabled
        {
            get
            {
                return false;
            }

            set
            {
                ;
            }
        }

        public int LineCount
        {
            get
            {
                return _textBox.LineCount;
            }
        }



        public int SelectionLength
        {
            get
            {
                return _textBox.SelectionEnd - _textBox.SelectionStart;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Text
        {
            get
            {
                return _textBox.Text;
            }

            set
            {
                _textBox.Text = value;
            }
        }

        public event EventHandler<object> SelectionChanged;
        public event EventHandler<object> TextChanged;

        public void Focus()
        {
            _textBox.RequestFocus();
        }

        public int GetCharacterIndexFromLineIndex(int currentLineIndex)
        {
            return 7;
        }

        public int GetLineIndexFromCharacterIndex(int rawPos)
        {
            return 8;
        }

        public int GetLineLength(int currentLineIndex)
        {
            return 9;
        }

        public void Select(int indexInSourceText, int length)
        {
            _textBox.SetSelection(indexInSourceText, length);
        }

        public void SelectAll()
        {
            _textBox.SelectAll();
        }
    }
}