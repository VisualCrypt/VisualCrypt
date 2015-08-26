using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Language;

namespace VisualCrypt.Desktop.Views
{
  
    public sealed partial class SetPasswordDialog : INotifyPropertyChanged
    {
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;
        readonly IParamsProvider _paramsProvider;
	    readonly Action<bool> _setIsPasswordSet;
	    readonly bool _isPasswordSet;

        public SetPasswordDialog()
        {
            _messageBoxService = Service.Get<IMessageBoxService>();
            _encryptionService = Service.Get<IEncryptionService>();
            _paramsProvider = Service.Get<IParamsProvider>();

            InitializeComponent();
            DataContext = this;

            PwBox.TextChanged += PwBox_TextChanged;
            PwBox.Text = string.Empty;
            PwBox_TextChanged(null, null);

            PwBox.Focus();

            PreviewKeyDown += CloseWithEscape;

			var parameters = _paramsProvider.GetParams<SetPasswordDialog,Tuple<SetPasswordDialogMode,Action<bool>,bool>>();
            SetMode(parameters.Item1);
	        _setIsPasswordSet = parameters.Item2;
	        _isPasswordSet = parameters.Item3;
        }

        void PwBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var response = _encryptionService.SanitizePassword(PwBox.Text);
                if (response.IsSuccess)
                {
                    var sigCount = response.Result.Length;
                    SignificantCharCountText = string.Format("{0} of {1} Unicode Characters", sigCount.ToString("N0"),
                        NormalizedPassword.MaxSanitizedPasswordLength.ToString("N0"));
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
                        RequestButton.OKCancel,
                        RequestImage.Warning) == RequestResult.OK;
                    if (!okClicked)
                        return;
                }

                var setPasswordResponse = _encryptionService.SetPassword(PwBox.Text);
                if (!setPasswordResponse.IsSuccess)
                {
                    _setIsPasswordSet(false);
                    _messageBoxService.ShowError(setPasswordResponse.Error);
                    return;
                }
				_setIsPasswordSet(true);
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
					_setIsPasswordSet(false);
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
            if (PwBox.Text.Length > 0 || _isPasswordSet)
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