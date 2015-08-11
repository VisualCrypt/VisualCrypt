﻿using Windows.UI.Xaml;
using VisualCrypt.Cryptography.Portable.MVVM;
using VisualCrypt.Language;

namespace VisualCrypt.Windows.Models
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
            OnPropertyChanged(() => TextBlockClearPasswordVisibility);
            OnPropertyChanged(() => PasswordStatus);
            OnPropertyChanged(() => HyperlinkPasswordText);
            OnPropertyChanged(() => MenuPasswordText);
        }

        public bool IsPasswordSet
        {
            get { return _isPasswordSet; }
            set
            {
                _isPasswordSet = value;
                RaiseAllChanged();
            }
        }

        bool _isPasswordSet;

        public Visibility TextBlockClearPasswordVisibility
        {
            get { return IsPasswordSet ? Visibility.Visible : Visibility.Collapsed; }
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
    }
}
