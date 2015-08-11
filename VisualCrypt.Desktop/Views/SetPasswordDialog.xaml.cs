using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.PrismSupport;
using VisualCrypt.Desktop.Shared.Services;
using VisualCrypt.Language;

namespace VisualCrypt.Desktop.Views
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed partial class SetPasswordDialog : INotifyPropertyChanged
    {
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;

        [ImportingConstructor]
        public SetPasswordDialog(IMessageBoxService messageBoxService,
            IEncryptionService encryptionService, IParamsProvider paramsProvider)
        {
            _messageBoxService = messageBoxService;
            _encryptionService = encryptionService;

            InitializeComponent();
            DataContext = this;

            PwBox.TextChanged += PwBox_TextChanged;
            PwBox.Text = string.Empty;
            PwBox_TextChanged(null, null);

            PwBox.Focus();

            PreviewKeyDown += CloseWithEscape;
            SetMode(paramsProvider.GetParams<SetPasswordDialog, SetPasswordDialogMode>());
        }

        void PwBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                var response = _encryptionService.SanitizePassword(PwBox.Text);
                if (response.IsSuccess)
                {
                    var sigCount = response.Result.Length;
                    SignificantCharCountText = string.Format("{0} of {1} Unicode Characters", sigCount.ToString("N0"),
                        PrunedPassword.MaxSanitizedPasswordLength.ToString("N0"));
                }
                else
                {
                    _messageBoxService.ShowError(response.Error);
                }
            }
            catch (Exception ex)
            {
                _messageBoxService.ShowError(ex);
            }
        }


        void SetMode(SetPasswordDialogMode setPasswordDialogMode)
        {
            switch (setPasswordDialogMode)
            {
                case SetPasswordDialogMode.Set:
                    Title = "Set Password";
                    ButtonOk.Content = "Set Password";
                    break;
                case SetPasswordDialogMode.Change:
                    Title = "Change Password";
                    ButtonOk.Content = "Change Password";
                    break;
                case SetPasswordDialogMode.SetAndEncrypt:
                    Title = "Set Password & Encrypt";
                    ButtonOk.Content = "Encrypt";
                    break;
                case SetPasswordDialogMode.SetAndDecrypt:
                    Title = "Set Password & Decrypt";
                    ButtonOk.Content = "Decrypt";
                    break;
                case SetPasswordDialogMode.SetAndEncryptAndSave:
                    Title = "Set Password, Encrypt and Save";
                    ButtonOk.Content = "Encrypt and Save";
                    break;
                case SetPasswordDialogMode.SetAndDecryptLoadedFile:
                    Title = "Enter password to decrypt loaded file";
                    ButtonOk.Content = "Decrypt loaded file";
                    break;
                case SetPasswordDialogMode.CorrectPassword:
                    Title = "The current password is not correct";
                    ButtonOk.Content = "Change Password and decrypt";
                    break;
            }
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

        DelegateCommand _setPasswordCommand;

        void ExecuteSetPasswordCommand()
        {
            try
            {
                var response = _encryptionService.SanitizePassword(PwBox.Text);
                if (!response.IsSuccess)
                {
                    _messageBoxService.ShowError(response.Error);
                    return;
                }

                var sigCount = response.Result.Length;
                if (sigCount == 0)
                {
                    string warningMessage = PwBox.Text.Length == sigCount
                        ? "Use empty password?"
                        : "The password is effectively empty - are you sure?";
                    var okClicked = _messageBoxService.Show(warningMessage, "Use empty password?",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning) == MessageBoxResult.OK;
                    if (!okClicked)
                        return;
                }

                var setPasswordResponse = _encryptionService.SetPassword(PwBox.Text);
                if (!setPasswordResponse.IsSuccess)
                {
                    PasswordManager.PasswordInfo.IsPasswordSet = false;
                    _messageBoxService.ShowError(setPasswordResponse.Error);
                    return;
                }
                PasswordManager.PasswordInfo.IsPasswordSet = true;
                DialogResult = true;
                Close();
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
            finally
            {
                PwBox.Text = string.Empty;
            }
        }

        bool CanExecuteSetPasswordCommand()
        {
            return true;
        }

        DelegateCommand _clearPasswordCommand;

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

        public string SignificantCharCountText
        {
            get { return _significantCharCountText; }
            set
            {
                _significantCharCountText = value;
                OnPropertyChanged();
            }
        }

        string _significantCharCountText;

        void ExecuteClearPasswordCommand()
        {
            try
            {
                var setPasswordResponse = _encryptionService.SetPassword(string.Empty);
                if (setPasswordResponse.IsSuccess)
                {
                    PasswordManager.PasswordInfo.IsPasswordSet = false;
                    PwBox.Text = string.Empty;
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        bool CanExecuteClearPasswordCommand()
        {
            if (PwBox.Text.Length > 0 || PasswordManager.PasswordInfo.IsPasswordSet)
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


        void Hyperlink_CreatePassword_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = _encryptionService.GenerateRandomPassword();
                if (response.IsSuccess)
                    PwBox.Text = response.Result;
                else
                    _messageBoxService.ShowError(response.Error);
            }
            catch (Exception ex)
            {
                _messageBoxService.ShowError(ex);
            }
        }


        void Hyperlink_ReadPWSpec_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (
                    var process = new Process
                    {
                        StartInfo = {UseShellExecute = true, FileName = Loc.Strings.uriPWSpecUrl}
                    })
                    process.Start();
            }
            catch (Exception ex)
            {
                _messageBoxService.ShowError(ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}