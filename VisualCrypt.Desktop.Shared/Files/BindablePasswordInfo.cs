using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Mvvm;

namespace VisualCrypt.Desktop.Shared.Files
{
    public class BindablePasswordInfo : BindableBase
    {
        public bool IsPasswordSet
        {
            get { return _isPasswordSet; }
            set
            {
                if (_isPasswordSet == value)
                    return;
                _isPasswordSet = value;
                OnPropertyChanged(() => IsPasswordSet);
                OnPropertyChanged(() => TextBlockClearPasswordVisibility);
                OnPropertyChanged(() => PasswordStatus);
                OnPropertyChanged(()=>HyperlinkPasswordText);
            }
        }
        bool _isPasswordSet;

        public Visibility TextBlockClearPasswordVisibility
        {
            get { return IsPasswordSet ? Visibility.Visible : Visibility.Collapsed; }

        }

        public string PasswordStatus
        {
            get { return _isPasswordSet ? new string('\u25CF' /* 'BLACK CIRCLE' */, 5) : ""; }
        }

        public string HyperlinkPasswordText
        {
            get { return _isPasswordSet ? "Change Password" : "Set Password"; }
        }
        public string MenuPasswordText
        {
            get { return _isPasswordSet ? "Change _Password..." : "Set _Password..."; }
        }
    }
}
