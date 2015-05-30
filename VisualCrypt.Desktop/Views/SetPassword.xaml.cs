﻿using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using VisualCrypt.Cryptography.Net.Tools;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.Views
{
    public sealed partial class SetPassword
    {
        readonly IMessageBoxService _messageBoxService;
        readonly IEncryptionService _encryptionService;

        public SetPassword(SetPasswordDialogMode setPasswordDialogMode, IMessageBoxService messageBoxService,
            IEncryptionService encryptionService)
        {
            _messageBoxService = messageBoxService;
            _encryptionService = encryptionService;

            InitializeComponent();
            DataContext = this;


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
                    Title = "Set Password & Decrpyt";
                    ButtonOk.Content = "Decrypt";
                    break;
            }

            PwBox.Focus();

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

        DelegateCommand _setPasswordCommand;

        void ExecuteSetPasswordCommand()
        {
            try
            {
                var setPasswordResponse = _encryptionService.SetPassword(PwBox.SecurePassword.ToUTF16ByteArray());
                if (!setPasswordResponse.Success)
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
                PwBox.Password = string.Empty;
                PwBox2.Password = string.Empty;
            }
        }

        bool CanExecuteSetPasswordCommand()
        {
            if (PwBox.SecurePassword.IsEqualTo(PwBox2.SecurePassword))
                return true;
            return false;
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

        void ExecuteClearPasswordCommand()
        {
            PwBox.SecurePassword.Clear();
            PwBox.Password = string.Empty;
            PwBox2.SecurePassword.Clear();
            PwBox2.Password = string.Empty;
        }

        bool CanExecuteClearPasswordCommand()
        {
            if (PwBox.SecurePassword.Length > 0 || PwBox2.SecurePassword.Length > 0)
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