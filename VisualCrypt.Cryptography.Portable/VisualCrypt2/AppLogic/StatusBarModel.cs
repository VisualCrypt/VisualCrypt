using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Language;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic
{
    public class StatusBarModel : NotifyPropertyChanged
    {
        public bool IsPlaintextBarHidden
        {
            get { return _isPlaintextBarHidden; }
            set
            {
                _isPlaintextBarHidden = value;
                OnPropertyChanged();
            }
        }
        bool _isPlaintextBarHidden;

        public bool IsProgressBarHidden
        {
            get { return _isProgressBarHidden; }
            set
            {
                _isProgressBarHidden = value;
                OnPropertyChanged();
            }
        }

        bool _isProgressBarHidden;

        public bool IsEncryptedBarHidden
        {
            get { return _isEncryptedBarHidden; }
            set
            {
                _isEncryptedBarHidden = value;
                OnPropertyChanged();
            }
        }
        bool _isEncryptedBarHidden;

        public int ProgressPercent
        {
            get { return _progressPercent; }
            set
            {
                _progressPercent = value;
                OnPropertyChanged();
            }
        }

        int _progressPercent;

        public string ProgressMessage
        {
            get { return _progressMessage; }
            set
            {
                _progressMessage = value;
                OnPropertyChanged();
            }
        }

        string _progressMessage;

        public string ProgressBarOpName
        {
            get { return _progressBarOpName; }
            set
            {
                _progressBarOpName = value;
                OnPropertyChanged();
            }
        }

        string _progressBarOpName;

        public string EncrytedBarText
        {
            get { return _encryptedBarText; }
            set
            {
                _encryptedBarText = value;
                OnPropertyChanged();
            }
        }

        string _encryptedBarText;

        public string StatusBarText
        {
            get { return _statusBarText; }
            private set
            {
                if (_statusBarText == value) return;
                _statusBarText = value;
                OnPropertyChanged();
            }
        }
        string _statusBarText;


        public void UpdateEncryptedBarText(FileModel filemodel)
        {
            string text = string.Format(Loc.Strings.encrpytedStatusbarText, CipherV2.Version,
                filemodel.CipherV2.RoundsExponent.Value, filemodel.VisualCryptText.Length);
            EncrytedBarText = text;
        }

        public void UpdateStatusBarText(string statusBarText)
        {
            StatusBarText = statusBarText;
        }

        public void ShowProgressBar(string description)
        {
            IsProgressBarHidden = false;
            IsPlaintextBarHidden = true;
            IsEncryptedBarHidden = true;
            ProgressBarOpName = description;
        }

        public void ShowPlainTextBar()
        {
            IsProgressBarHidden = true;
            IsPlaintextBarHidden = false;
            IsEncryptedBarHidden = true;
        }

        public void ShowEncryptedBar()
        {
            IsProgressBarHidden = true;
            IsPlaintextBarHidden = true;
            IsEncryptedBarHidden = false;
        }
    }
}
