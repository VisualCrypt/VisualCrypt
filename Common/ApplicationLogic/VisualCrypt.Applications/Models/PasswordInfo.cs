using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Applications.Models
{
    public class PasswordInfo : ViewModelBase
    {
        ResourceWrapper _res;
        public PasswordInfo()
        {
            _res = Service.Get<ResourceWrapper>();
            _res.Info.CultureChanged += (sender, args) => RaiseAllChanged();
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
                    ? Language.Strings.Resources.termPassword + " " + new string('\u25CF' /* 'BLACK CIRCLE' */, 5)
                    : Language.Strings.Resources.termSetPassword + "...";
            }
        }

        public string HyperlinkPasswordText
        {
            get { return _isPasswordSet ? "Change Password" : "Set Password"; }
        }

        public string MenuPasswordText
        {
            get { return _isPasswordSet ? Language.Strings.Resources.miVCChangePassword : Language.Strings.Resources.miVCSetPassword; }
        }

        public void SetIsPasswordSet(bool isPasswordSet)
        {
            IsPasswordSet = isPasswordSet;
        }
    }
}
