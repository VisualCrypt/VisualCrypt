using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Applications.Models
{
    public class PasswordInfo : ViewModelBase
    {
        ResourceWrapper _resourceWrapper;
        public PasswordInfo()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _resourceWrapper.Info.CultureChanged += (sender, args) => RaiseAllChanged();
        }

        void RaiseAllChanged()
        {
            OnPropertyChanged(() => IsPasswordSet);
            OnPropertyChanged(() => IsTextBlockClearPasswordVisible);
            OnPropertyChanged(() => PasswordStatus);
            OnPropertyChanged(() => HyperlinkPasswordText);
            OnPropertyChanged(() => MenuPasswordText);
        }

        public bool IsPasswordSet
        {
            get { return _isPasswordSet; }
            private set
            {
                _isPasswordSet = value;
                RaiseAllChanged();
            }
        }

        bool _isPasswordSet;

        public bool IsTextBlockClearPasswordVisible
        {
            get { return !IsPasswordSet; }
        }

        public string PasswordStatus
        {
            get
            {
                return _isPasswordSet
                    ? Service.Get<ResourceWrapper>().termPassword + " " + new string('\u25CF' /* 'BLACK CIRCLE' */, 5)
                    : Service.Get<ResourceWrapper>().miVCSetPassword;
            }
        }

        public string HyperlinkPasswordText
        {
            get { return _isPasswordSet 
                    ? Service.Get<ResourceWrapper>().termChangePassword
                    : Service.Get<ResourceWrapper>().termSetPassword;
            }
        }

        public string MenuPasswordText
        {
            get { return _isPasswordSet ? Service.Get<ResourceWrapper>().miVCChangePassword : Service.Get<ResourceWrapper>().miVCSetPassword; }
        }

        public void SetIsPasswordSet(bool isPasswordSet)
        {
            IsPasswordSet = isPasswordSet;
        }
    }
}
