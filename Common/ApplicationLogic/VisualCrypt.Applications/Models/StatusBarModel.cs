using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Language;

namespace VisualCrypt.Applications.Models
{
    public class StatusBarModel : ViewModelBase
    {
        public bool IsPlaintextBarVisible
        {
            get { return _isPlaintextBarVisible; }
            set
            {
                _isPlaintextBarVisible = value;
                OnPropertyChanged();
            }
        }
        bool _isPlaintextBarVisible;

        public bool IsProgressBarVisible
        {
            get { return _isProgressBarVisible; }
            set
            {
                _isProgressBarVisible = value;
                OnPropertyChanged();
            }
        }

        bool _isProgressBarVisible;

        public bool IsEncryptedBarVisible
        {
            get { return _isEncryptedBarVisible; }
            set
            {
                _isEncryptedBarVisible = value;
                OnPropertyChanged();
            }
        }
        bool _isEncryptedBarVisible;

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
            IsProgressBarVisible = true;
            IsPlaintextBarVisible = false;
            IsEncryptedBarVisible = false;
            ProgressBarOpName = description;
        }

        public void ShowPlainTextBar()
        {
            IsProgressBarVisible = false;
            IsPlaintextBarVisible = true;
            IsEncryptedBarVisible = false;
        }

        public void ShowEncryptedBar()
        {
            IsProgressBarVisible = false;
            IsPlaintextBarVisible = false;
            IsEncryptedBarVisible = true;
        }

	    public void OnFileModelChanged(FileModel fileModel)
	    {
		    if (fileModel.IsEncrypted)
		    {
			    ShowEncryptedBar();
			    UpdateEncryptedBarText(fileModel);
		    }
		    else
		    {
			    ShowPlainTextBar();
				
		    }
	    }
    }
}
