using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class RenameContentDialog : ContentDialog, INotifyPropertyChanged
    {
        public RenameContentDialog()
        {
            this.InitializeComponent();
        }

        public StorageFile StorageFile;

        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_filename != value && value != null)
                {
                    _filename = value.Trim();
                    if (_filename.Contains("."))
                        _filename = _filename.Remove(value.IndexOf('.'));

                    if (!IsFilenameValid(_filename))
                        IsPrimaryButtonEnabled = false;

                    OnPropertyChanged();
                }
            }
        }
        string _filename = string.Empty;



        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //var newPathAndFilename = Path.Combine(Path.GetDirectoryName(StorageFile.Path), _filename + ".visualcrypt");
            //if (File.Exists(newPathAndFilename))
            //    args.Cancel = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (!IsFilenameValid(textBox.Text))
            {
                IsPrimaryButtonEnabled = false;
                return;
            }
            IsPrimaryButtonEnabled = true;
        }

        bool IsFilenameValid(string newFilename)
        {
            if (string.IsNullOrWhiteSpace(newFilename))
                return false;
            List<char> disallowedChars = new List<char>();
            disallowedChars.Add(Path.AltDirectorySeparatorChar);
            disallowedChars.Add(Path.DirectorySeparatorChar);
            disallowedChars.Add(Path.VolumeSeparatorChar);
            disallowedChars.Add(Path.PathSeparator);
            disallowedChars.AddRange(Path.GetInvalidFileNameChars());
            disallowedChars.AddRange(Path.GetInvalidPathChars());
            disallowedChars.Add('.');
            foreach (var c in newFilename)
            {
                if (disallowedChars.Contains(c))
                    return false;
            }

            return true;

        }
    }
}
