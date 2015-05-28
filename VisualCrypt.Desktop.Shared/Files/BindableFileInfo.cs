using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Desktop.Shared.Files
{
    public class BindableFileInfo : INotifyPropertyChanged
    {
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                OnPropertyChanged();
            }
        }
        bool _isDirty;

        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                OnPropertyChanged();
            }
        }
         string _filename;

        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set
            {
                _isEncrypted = value;
                OnPropertyChanged();
            }
        }
        bool _isEncrypted;

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
