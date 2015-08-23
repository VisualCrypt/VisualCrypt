using VisualCrypt.Applications.Apps.MVVM;
using VisualCrypt.Language;

namespace VisualCrypt.Applications.Apps.Models
{
    public class PasswordInfo : ViewModelBase
    {
        public PasswordInfo()
        {
            Loc.LocaleChanged += (sender, args) => RaiseAllChanged();
        }

        void RaiseAllChanged()
        {
            OnPropertyChanged(() => IsPasswordSet);
            OnPropertyChanged(() => IsTextBlockClearPasswordHidden);
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

        public bool IsTextBlockClearPasswordHidden
        {
            get { return !IsPasswordSet; }
        }

        public string PasswordStatus
        {
            get
            {
                return _isPasswordSet
                    ? Loc.Strings.termPassword + " " + new string('\u25CF' /* 'BLACK CIRCLE' */, 5)
                    : Loc.Strings.termSetPassword + "...";
            }
        }

        public string HyperlinkPasswordText
        {
            get { return _isPasswordSet ? "Change Password" : "Set Password"; }
        }

        public string MenuPasswordText
        {
            get { return _isPasswordSet ? Loc.Strings.miVCChangePassword : Loc.Strings.miVCSetPassword; }
        }

        public void SetIsPasswordSet(bool isPasswordSet)
        {
            IsPasswordSet = isPasswordSet;
        }
    }
}
