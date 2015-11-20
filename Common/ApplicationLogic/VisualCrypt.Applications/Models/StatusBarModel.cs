using System.Reflection;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Applications.Models
{
    public class StatusBarModel : ViewModelBase
    {
        readonly ResourceWrapper _resourceWrapper;

        public StatusBarModel()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }

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
                if (value.Length > 0 && char.IsNumber(value[0]))
                {
                    _progressMessage = string.Format("{0} Bytes", value);
                    OnPropertyChanged();
                    return;
                }

                switch(value)
                {
                    case "":
                        _progressMessage = string.Empty;
                        break;
                    case LocalizableStrings.MsgCalculatingMAC:
                        _progressMessage = _resourceWrapper.encProgr_CalculatingMAC;
                        break;
                    case LocalizableStrings.MsgDecryptingMAC:
                        _progressMessage = _resourceWrapper.encProgr_DecryptingMAC;
                        break;
                    case LocalizableStrings.MsgDecryptingMessage:
                        _progressMessage = _resourceWrapper.encProgr_DecryptingMessage;
                        break;

                    case LocalizableStrings.MsgDecryptingRandomKey:
                        _progressMessage = _resourceWrapper.encProgr_DecryptingRandomKey;
                        break;
                    case LocalizableStrings.MsgEncryptingMAC:
                        _progressMessage = _resourceWrapper.encProgr_EncryptingMAC;
                        break;
                    case LocalizableStrings.MsgEncryptingMessage:
                        _progressMessage = _resourceWrapper.encProgr_EncryptingMessage;
                        break;
                    case LocalizableStrings.MsgEncryptingRandomKey:
                        _progressMessage = _resourceWrapper.encProgr_EncryptingRandomKey;
                        break;
                    case LocalizableStrings.MsgProcessingKey:
                        _progressMessage = _resourceWrapper.encProgr_ProcessingKey;
                        break;
                    case LocalizableStrings.MsgFileLoading:
                        _progressMessage = _resourceWrapper.fileProgr_Loading;
                        break;
                    case LocalizableStrings.MsgFileSaving:
                        _progressMessage = _resourceWrapper.fileProgr_Saving;
                        break;
                    case LocalizableStrings.MsgAnalyzingContents:
                        _progressMessage = _resourceWrapper.fileProg_MsgAnalyzingContents;
                        break;
                    default:
                        _progressMessage = "LocalizableString: Missing";
                        break;
                }
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
            string text = string.Format(_resourceWrapper.encrpytedStatusbarText, CipherV2.Version,
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
