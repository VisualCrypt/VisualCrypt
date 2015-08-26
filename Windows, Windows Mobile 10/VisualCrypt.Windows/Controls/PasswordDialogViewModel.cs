using System;
using System.ComponentModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Documents;
using Prism.Commands;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Language;
using VisualCrypt.Windows.Services;

namespace VisualCrypt.Windows.Controls
{
    class PasswordDialogViewModel : ViewModelBase, IActiveCleanup
    {
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;
        readonly Action<bool> _closePopup;
        readonly Action<bool> _setIsPasswordSet;
        readonly bool _isPasswordSetWhenDialogOpened;

        public PasswordDialogViewModel(SetPasswordDialogMode setPasswordDialogMode, 
            Action<bool> closePopup, Action<bool> setIsPasswordSet, bool isPasswordSetWhenDialogOpened)
        {
            _encryptionService = Service.Get<IEncryptionService>();
            _messageBoxService = Service.Get<IMessageBoxService>();

            _closePopup = closePopup;
            _setIsPasswordSet = setIsPasswordSet;
            _isPasswordSetWhenDialogOpened = isPasswordSetWhenDialogOpened;
            PropertyChanged += OnPasswordBoxTextChanged;

            SetMode(setPasswordDialogMode);
        }

        #region Bound Properties

        public string PWSpecUrl => Loc.Strings.uriPWSpecUrl;

        public string PasswordBoxText
        {
            get { return _passwordBoxText; }
            set
            {
                _passwordBoxText = value;
                OnPropertyChanged(nameof(PasswordBoxText));
            }
        }
        string _passwordBoxText = string.Empty;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        string _title = string.Empty;

        public string OKButtonContent
        {
            get { return _okButtonContent; }
            set
            {
                _okButtonContent = value;
                OnPropertyChanged(nameof(OKButtonContent));
            }
        }
        string _okButtonContent = string.Empty;

        public string SignificantCharCountText
        {
            get { return _significantCharCountText; }
            set
            {
                _significantCharCountText = value;
                OnPropertyChanged(nameof(SignificantCharCountText));
            }
        }
        string _significantCharCountText;

        #endregion

        #region Event handlers
        void OnPasswordBoxTextChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(PasswordBoxText))
                return;

            try
            {
                var response = _encryptionService.SanitizePassword(_passwordBoxText);
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

            SetPasswordCommand.RaiseCanExecuteChanged();
            ClearPasswordCommand.RaiseCanExecuteChanged();
        }

        public void Hyperlink_CreatePassword_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            try
            {
                var response = _encryptionService.GenerateRandomPassword();
                if (response.IsSuccess)
                    PasswordBoxText = response.Result;
                else
                    _messageBoxService.ShowError(response.Error);
            }
            catch (Exception ex)
            {
                _messageBoxService.ShowError(ex);
            }
        }

        #endregion

        #region private methods

        void SetMode(SetPasswordDialogMode setPasswordDialogMode)
        {
            switch (setPasswordDialogMode)
            {
                case SetPasswordDialogMode.Set:
                    Title = "Set Password";
                    OKButtonContent = "Set Password";
                    break;
                case SetPasswordDialogMode.Change:
                    Title = "Change Password";
                    OKButtonContent = "Change Password";
                    break;
                case SetPasswordDialogMode.SetAndEncrypt:
                    Title = "Set Password & Encrypt";
                    OKButtonContent = "Encrypt";
                    break;
                case SetPasswordDialogMode.SetAndDecrypt:
                    Title = "Set Password & Decrypt";
                    OKButtonContent = "Decrypt";
                    break;
                case SetPasswordDialogMode.SetAndEncryptAndSave:
                    Title = "Set Password, Encrypt and Save";
                    OKButtonContent = "Encrypt and Save";
                    break;
                case SetPasswordDialogMode.SetAndDecryptLoadedFile:
                    Title = "Enter password to decrypt loaded file";
                    OKButtonContent = "Decrypt loaded file";
                    break;
                case SetPasswordDialogMode.CorrectPassword:
                    Title = "The current password is not correct";
                    OKButtonContent = "Change Password and decrypt";
                    break;
            }
        }

        void CloseWithEscape(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Escape)
            //    Close();
        }

        void Close(bool setClicked)
        {
            _closePopup(setClicked);
        }

        #endregion

        #region SetPasswordCommand

        public DelegateCommand SetPasswordCommand
            => CreateCommand(ref _setPasswordCommand, ExecuteSetPasswordCommand, () => true);
        DelegateCommand _setPasswordCommand;

        void ExecuteSetPasswordCommand()
        {
            try
            {
                var response = _encryptionService.SanitizePassword(_passwordBoxText);
                if (!response.IsSuccess)
                {
                    _messageBoxService.ShowError(response.Error);
                    return;
                }

                var sigCount = response.Result.Length;
                if (sigCount == 0)
                {
                    string warningMessage = _passwordBoxText.Length == sigCount
                        ? "Use empty password?"
                        : "The password is effectively empty - are you sure?";
                    var okClicked = _messageBoxService.Show(warningMessage, "Use empty password?", RequestButton.OKCancel,
                        RequestImage.Warning) == RequestResult.OK;
                    if (!okClicked)
                        return;
                }

                var setPasswordResponse = _encryptionService.SetPassword(_passwordBoxText);
                if (!setPasswordResponse.IsSuccess)
                {
                    _setIsPasswordSet(false);
                    _messageBoxService.ShowError(setPasswordResponse.Error);
                    return;
                }
                _setIsPasswordSet(true);
                Close(true);
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
            finally
            {
                _passwordBoxText = string.Empty;
            }
        }

        #endregion

        #region ClearPasswordCommand

        public DelegateCommand ClearPasswordCommand
            => CreateCommand(ref _clearPasswordCommand, ExecuteClearPasswordCommand, CanExecuteClearPasswordCommand);
        DelegateCommand _clearPasswordCommand;

        void ExecuteClearPasswordCommand()
        {
            try
            {
                var setPasswordResponse = _encryptionService.SetPassword(string.Empty);
                if (setPasswordResponse.IsSuccess)
                {
                    _setIsPasswordSet(false);
                    _passwordBoxText = string.Empty;
                    Close(false);
                }
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }
        bool CanExecuteClearPasswordCommand()
        {
            if (_passwordBoxText.Length > 0 || _isPasswordSetWhenDialogOpened)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region CancelCommand

        public DelegateCommand CancelCommand
            => CreateCommand(ref _cancelCommand, ExecuteCancelCommand, () => true);
        DelegateCommand _cancelCommand;

        void ExecuteCancelCommand()
        {
            Close(false);
        }

        #endregion


        public void Cleanup()
        {
            PropertyChanged -= OnPasswordBoxTextChanged;
        }
    }
}
