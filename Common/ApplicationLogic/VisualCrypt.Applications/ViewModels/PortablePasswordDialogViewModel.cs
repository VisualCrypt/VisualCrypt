using Prism.Commands;
using System;
using System.ComponentModel;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Applications.ViewModels
{
    public class PortablePasswordDialogViewModel : ViewModelBase
    {
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;
        readonly ResourceWrapper _resourceWrapper;
        readonly IPrinter _printer;

        Action<bool> _setIsPasswordSet;
        Action<bool> _closeAction;
        bool _isPasswordSetWhenDialogOpened;

        public ResourceWrapper ResourceWrapper { get { return _resourceWrapper; } }

        public PortablePasswordDialogViewModel()
        {
            _encryptionService = Service.Get<IEncryptionService>();
            _messageBoxService = Service.Get<IMessageBoxService>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _printer = Service.Get<IPrinter>();

           

            PropertyChanged += OnPasswordBoxTextChanged;

          
            OnPasswordBoxTextChanged(null, new PropertyChangedEventArgs(nameof(PasswordBoxText)));
        }

        public void Init(SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, Action<bool> closeAction, bool isPasswordSetWhenDialogOpened)
        {
            _setIsPasswordSet = setIsPasswordSet;
            _closeAction = closeAction;
            _isPasswordSetWhenDialogOpened = isPasswordSetWhenDialogOpened;
            SetMode(setPasswordDialogMode);
        }

        #region Bound Properties

        public string PWSpecUrl => _resourceWrapper.uriPWSpecUrl;

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
                    SignificantCharCountText = string.Format(_resourceWrapper.spd_msgXofYUnicodeChars, sigCount.ToString("N0"));
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
        }

     

        #endregion

        #region private Methods

        void SetMode(SetPasswordDialogMode setPasswordDialogMode)
        {
            switch (setPasswordDialogMode)
            {
                case SetPasswordDialogMode.Set:
                    Title = _resourceWrapper.spdm_Set_Title;
                    OKButtonContent = _resourceWrapper.spdm_Set_OK;
                    break;
                case SetPasswordDialogMode.Change:
                    Title = _resourceWrapper.spdm_Change_Title;
                    OKButtonContent = _resourceWrapper.spdm_Change_OK;
                    break;
                case SetPasswordDialogMode.SetAndEncrypt:
                    Title = _resourceWrapper.spdm_SetAndEncrypt_Title;
                    OKButtonContent = _resourceWrapper.spdm_SetAndEncrypt_OK;
                    break;
                case SetPasswordDialogMode.SetAndDecrypt:
                    Title = _resourceWrapper.spdm_SetAndDecrypt_Title;
                    OKButtonContent = _resourceWrapper.spdm_SetAndDecrypt_OK;
                    break;
                case SetPasswordDialogMode.SetAndEncryptAndSave:
                    Title = _resourceWrapper.spdm_SetAndEncryptAndSave_Title;
                    OKButtonContent = _resourceWrapper.spdm_SetAndEncryptAndSave_OK;
                    break;
                case SetPasswordDialogMode.SetAndDecryptLoadedFile:
                    Title = _resourceWrapper.spdm_SetAndDecryptLoadedFile_Title;
                    OKButtonContent = _resourceWrapper.spdm_SetAndDecryptLoadedFile_OK;
                    break;
                case SetPasswordDialogMode.CorrectPassword:
                    Title = _resourceWrapper.spdm_CorrectPassword_Title;
                    OKButtonContent = _resourceWrapper.spdm_CorrectPassword_OK;
                    break;
            }
        }

        public void Close(bool setClicked)
        {
            _closeAction(setClicked);
        }

        #endregion

        #region SetPasswordCommand

        public DelegateCommand SetPasswordCommand
            => CreateCommand(ref _setPasswordCommand, ExecuteSetPasswordCommand, () => true);
        DelegateCommand _setPasswordCommand;

        async void ExecuteSetPasswordCommand()
        {
            try
            {
                var response = _encryptionService.SanitizePassword(_passwordBoxText);
                if (!response.IsSuccess)
                {
                    await _messageBoxService.ShowError(response.Error);
                    return;
                }

                var sigCount = response.Result.Length;
                if (sigCount == 0)
                {
                    string warningMessage = _passwordBoxText.Length == sigCount
                         ? _resourceWrapper.spd_msgUseEmptyPassword
                         : _resourceWrapper.spd_msgPasswordEffectivelyEmpty;
                    var okClicked = await _messageBoxService.Show(warningMessage, _resourceWrapper.spd_msgUseEmptyPassword,
                        RequestButton.OKCancel,
                        RequestImage.Warning) == RequestResult.OK;
                    if (!okClicked)
                        return;
                }

                var setPasswordResponse = _encryptionService.SetPassword(_passwordBoxText);
                if (!setPasswordResponse.IsSuccess)
                {
                    _setIsPasswordSet(false);
                    await _messageBoxService.ShowError(setPasswordResponse.Error);
                    return;
                }
                _setIsPasswordSet(true);
                Close(true);
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e);
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

        #region SuggestPasswordCommand

        public DelegateCommand SuggestPasswordCommand
            => CreateCommand(ref _suggestPasswordCommandPasswordCommand, ExecuteSuggestPasswordCommand, () => true);
        DelegateCommand _suggestPasswordCommandPasswordCommand;

        void ExecuteSuggestPasswordCommand()
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

        #region PrintPasswordCommand

        public DelegateCommand PrintPasswordCommand
            => CreateCommand(ref _printPasswordCommand, ExecutePrintPasswordCommand, CanExecutePrintPasswordCommand);
        DelegateCommand _printPasswordCommand;

        void ExecutePrintPasswordCommand()
        {
            try
            {
                var response = _encryptionService.SanitizePassword(_passwordBoxText);
                if (response.IsSuccess)
                {
                    string normalizedPassword = response.Result;
                    _printer.PrintEditorText(normalizedPassword);


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
        bool CanExecutePrintPasswordCommand()
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
            PasswordBoxText = string.Empty;
            Close(false);
        }

        #endregion

    }
}
