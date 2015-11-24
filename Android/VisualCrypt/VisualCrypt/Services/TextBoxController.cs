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
        public int CaretIndex
        {
            get
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsUndoEnabled
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int LineCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object PlatformTextBox
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public int SelectionLength
        {
            get
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<object> SelectionChanged;
        public event EventHandler<object> TextChanged;

        public void Focus()
        {
            throw new NotImplementedException();
        }

        public int GetCharacterIndexFromLineIndex(int currentLineIndex)
        {
            throw new NotImplementedException();
        }

        public int GetLineIndexFromCharacterIndex(int rawPos)
        {
            throw new NotImplementedException();
        }

        public int GetLineLength(int currentLineIndex)
        {
            throw new NotImplementedException();
        }

        public void Select(int indexInSourceText, int length)
        {
            throw new NotImplementedException();
        }

        public void SelectAll()
        {
            throw new NotImplementedException();
        }
    }
}