using System.Windows;
using System.Windows.Input;
using VisualCrypt.Desktop.Lib;
using VisualCrypt.Desktop.State;
using VisualCrypt.Net.Tools;
using VisualCrypt.Portable.APIV2.Interfaces;
using VisualCrypt.Portable.Editor.Enums;

namespace VisualCrypt.Desktop.Views
{
    public sealed partial class SetPassword
    {
        DelegateCommand _setPasswordCommand;
        DelegateCommand _clearPasswordCommand;
        readonly IVisualCryptAPIV2 _visualCyrptAPI;

        public SetPassword(SetPasswordDialogMode setPasswordDialogMode, IVisualCryptAPIV2 visualCyrptAPI)
        {
            InitializeComponent();
            DataContext = this;
            _visualCyrptAPI = visualCyrptAPI;

            switch (setPasswordDialogMode)
            {
                case SetPasswordDialogMode.Set:
                    Title = "Set Password";
                    ButtonOk.Content = "Set";
                    break;
                case SetPasswordDialogMode.SetAndEncrypt:
                    Title = "Set Password & Encrypt";
                    ButtonOk.Content = "Encrypt";
                    break;
                case SetPasswordDialogMode.SetAndDecrypt:
                    Title = "Set Password & Decrpyt";
                    ButtonOk.Content = "Decrypt";
                    break;
            }

            pwBox.Focus();
            
            PreviewKeyDown += CloseWithEscape;
        }

        void CloseWithEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public DelegateCommand SetPasswordCommand
        {
            get
            {
                if (_setPasswordCommand != null)
                    return _setPasswordCommand;
                _setPasswordCommand = new DelegateCommand(ExecuteSetPasswordCommand, CanExecuteSetPasswordCommand);
                return _setPasswordCommand;
            }
        }

        private void ExecuteSetPasswordCommand()
        {
            var hashResponse =  _visualCyrptAPI.CreateSHA256PW32(pwBox.SecurePassword.ToUTF16ByteArray());
            if (hashResponse.Success)
            {
                ModelState.Transient.SHA256PW32 = hashResponse.Result;
            }
            
               

            if (pwBox.SecurePassword.Length > 0)
                ModelState.Transient.IsSHA256PasswordHashPresent = true;
            else
                ModelState.Transient.IsSHA256PasswordHashPresent = false;
           
            pwBox.Password = string.Empty;
            pwBox2.Password = string.Empty;
            DialogResult = true;
            Close();
        }

        bool CanExecuteSetPasswordCommand()
        {
            if (pwBox.SecurePassword.IsEqualTo(pwBox2.SecurePassword))
                return true;
            return false;
        }

        public DelegateCommand ClearPasswordCommand
        {
            get
            {
                if (_clearPasswordCommand != null)
                    return _clearPasswordCommand;
                _clearPasswordCommand = new DelegateCommand(ExecuteClearPasswordCommand, CanExecuteClearPasswordCommand);
                return _clearPasswordCommand;
            }
        }

        public SetPasswordDialogMode SetPasswordDialogMode { get; set; }

        void ExecuteClearPasswordCommand()
        {
            pwBox.SecurePassword.Clear();
            pwBox.Password = string.Empty;
            pwBox2.SecurePassword.Clear();
            pwBox2.Password = string.Empty;
        }

        bool CanExecuteClearPasswordCommand()
        {
            if (pwBox.SecurePassword.Length > 0 || pwBox2.SecurePassword.Length > 0)
            {
                return true;
            }
            return false;
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        void Password_Changed(object sender, RoutedEventArgs e)
        {
            SetPasswordCommand.RaiseCanExecuteChanged();
            ClearPasswordCommand.RaiseCanExecuteChanged();
        }

        void Password2_Changed(object sender, RoutedEventArgs e)
        {
            SetPasswordCommand.RaiseCanExecuteChanged();
            ClearPasswordCommand.RaiseCanExecuteChanged();
        }
    }
}