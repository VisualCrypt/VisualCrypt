using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Desktop.Views
{

    public sealed partial class SetPasswordDialog : INotifyPropertyChanged
    {
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;
        readonly ResourceWrapper _resourceWrapper;
        readonly IPrinter _printer;
        readonly IParamsProvider _paramsProvider;
        readonly Action<bool> _setIsPasswordSet;
        bool _isSessionPasswordSet;
        public ResourceWrapper ResourceWrapper { get { return _resourceWrapper; } }

        public SetPasswordDialog()
        {
            _messageBoxService = Service.Get<IMessageBoxService>();
            _encryptionService = Service.Get<IEncryptionService>();
            _printer = Service.Get<IPrinter>();
            _paramsProvider = Service.Get<IParamsProvider>();
            _resourceWrapper = Service.Get<ResourceWrapper>();

            InitializeComponent();
            DataContext = this;

            PwBox.TextChanged += PwBox_TextChanged;
            PwBox.Text = string.Empty;
            PwBox_TextChanged(null, null);

            PwBox.Focus();

            PreviewKeyDown += CloseWithEscape;

            var parameters = _paramsProvider.GetParams<SetPasswordDialog, Tuple<SetPasswordDialogMode, Action<bool>, bool>>();
            SetMode(parameters.Item1);
            _setIsPasswordSet = parameters.Item2;
            _isSessionPasswordSet = parameters.Item3;
        }

        void PwBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SetPasswordCommand.RaiseCanExecuteChanged();

                IsHyperlinkClearPWBoxEnabled = PwBox.Text.Length > 0 ? true : false;

                var response = _encryptionService.SanitizePassword(PwBox.Text);
                if (response.IsSuccess)
                {
                    var sigCount = response.Result.Length;
                    SignificantCharCountText = string.Format(_resourceWrapper.spd_msgXofYUnicodeChars, sigCount.ToString("N0"),
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
                    Title = _resourceWrapper.spdm_Set_Title;
                    ButtonOk.Content = _resourceWrapper.spdm_Set_OK;
                    break;
                case SetPasswordDialogMode.Change:
                    Title = _resourceWrapper.spdm_Change_Title;
                    ButtonOk.Content = _resourceWrapper.spdm_Change_OK;
                    break;
                case SetPasswordDialogMode.SetAndEncrypt:
                    Title = _resourceWrapper.spdm_SetAndEncrypt_Title;
                    ButtonOk.Content = _resourceWrapper.spdm_SetAndEncrypt_OK;
                    break;
                case SetPasswordDialogMode.SetAndDecrypt:
                    Title = _resourceWrapper.spdm_SetAndDecrypt_Title;
                    ButtonOk.Content = _resourceWrapper.spdm_SetAndDecrypt_OK;
                    break;
                case SetPasswordDialogMode.SetAndEncryptAndSave:
                    Title = _resourceWrapper.spdm_SetAndEncryptAndSave_Title;
                    ButtonOk.Content = _resourceWrapper.spdm_SetAndEncryptAndSave_OK;
                    break;
                case SetPasswordDialogMode.SetAndDecryptLoadedFile:
                    Title = _resourceWrapper.spdm_SetAndDecryptLoadedFile_Title;
                    ButtonOk.Content = _resourceWrapper.spdm_SetAndDecryptLoadedFile_OK;
                    break;
                case SetPasswordDialogMode.CorrectPassword:
                    Title = _resourceWrapper.spdm_CorrectPassword_Title;
                    ButtonOk.Content = _resourceWrapper.spdm_CorrectPassword_OK;
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

        async void ExecuteSetPasswordCommand()
        {
            try
            {
                var response = _encryptionService.SanitizePassword(PwBox.Text);
                if (!response.IsSuccess)
                {
                    await _messageBoxService.ShowError(response.Error);
                    return;
                }

                var sigCount = response.Result.Length;
                if (sigCount == 0)
                {
                    string warningMessage = PwBox.Text.Length == sigCount
                        ? _resourceWrapper.spd_msgUseEmptyPassword
                        : _resourceWrapper.spd_msgPasswordEffectivelyEmpty;
                    var okClicked = await _messageBoxService.Show(warningMessage, _resourceWrapper.spd_msgUseEmptyPassword,
                        RequestButton.OKCancel,
                        RequestImage.Warning) == RequestResult.OK;
                    if (!okClicked)
                        return;
                }

                var setPasswordResponse = _encryptionService.SetPassword(PwBox.Text);
                if (!setPasswordResponse.IsSuccess)
                {
                    _setIsPasswordSet(false);
                    await _messageBoxService.ShowError(setPasswordResponse.Error);
                    return;
                }
                _setIsPasswordSet(true);
                DialogResult = true;
                Close();
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e);
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

        public bool IsHyperlinkClearPWBoxEnabled
        {
            get { return _isHyperlinkClearPWBoxEnabled; }
            set
            {
                _isHyperlinkClearPWBoxEnabled = value;
                OnPropertyChanged();
            }
        }
        bool _isHyperlinkClearPWBoxEnabled;

        void ExecuteClearPasswordCommand()
        {
            try
            {
                var setPasswordResponse = _encryptionService.SetPassword(string.Empty);
                if (setPasswordResponse.IsSuccess)
                {
                    _isSessionPasswordSet = false;
                    _setIsPasswordSet(false);
                    ClearPasswordCommand.RaiseCanExecuteChanged();
                }
            }
            catch (Exception e)
            {
                _messageBoxService.ShowError(e);
            }
        }

        bool CanExecuteClearPasswordCommand()
        {
            if (_isSessionPasswordSet)
            {
                return true;
            }
            return false;
        }
        void Hyperlink_ClearPWBox_Click(object sender, RoutedEventArgs e)
        {
            PwBox.Text = string.Empty;
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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
                        StartInfo = { UseShellExecute = true, FileName = _resourceWrapper.uriPWSpecUrl }
                    })
                    process.Start();
            }
            catch (Exception ex)
            {
                _messageBoxService.ShowError(ex);
            }
        }

        void Hyperlink_Print_Password_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = _encryptionService.SanitizePassword(PwBox.Text);
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

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}