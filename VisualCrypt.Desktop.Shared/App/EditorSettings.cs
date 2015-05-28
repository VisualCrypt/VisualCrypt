using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using VisualCrypt.Desktop.Shared.Properties;
using VisualCrypt.Desktop.Shared.Settings;

namespace VisualCrypt.Desktop.Shared.App
{
    /// <summary>
    /// Tracks the state of the editor. Some properties will be serialized, some not.
    /// </summary>
    [DataContract]
    public sealed class EditorSettings : INotifyPropertyChanged
    {
        [DataMember]
        public bool IsStatusBarChecked
        {
            get { return _isStatusBarChecked; }
            set
            {
                _isStatusBarChecked = value;
                OnPropertyChanged();
                AppState.SaveSettings();
            }
        }
        bool _isStatusBarChecked;

        [DataMember]
        public bool IsWordWrapChecked
        {
            get { return _isWordWrapChecked; }
            set
            {
                _isWordWrapChecked = value;
                OnPropertyChanged();
                AppState.SaveSettings();
            }
        }
        bool _isWordWrapChecked;

        [DataMember]
        public bool IsSpellCheckingChecked
        {
            get { return _isSpellCheckingChecked; }
            set
            {
                _isSpellCheckingChecked = value;
                OnPropertyChanged();
                AppState.SaveSettings();
            }
        }
        bool _isSpellCheckingChecked;

        [DataMember]
        public int PrintMargin 
        {
            get { return _printMargin; }
            set
            {
                _printMargin = value; 
                OnPropertyChanged();
                AppState.SaveSettings();
            }
        }
        int _printMargin;

        [DataMember]
        public FontSettings FontSettings { get; set; }

        public bool IsZoom100Checked
        {
            get { return _isZoom100Checked; }
            set
            {
                _isZoom100Checked = value;
                OnPropertyChanged();
            }
        }
        bool _isZoom100Checked;

        public string ZoomLevelMenuText
        {
            get { return _zoomLevelMenuText; }
            set
            {
                _zoomLevelMenuText = value;
                OnPropertyChanged();
            }
        }
        string _zoomLevelMenuText;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}