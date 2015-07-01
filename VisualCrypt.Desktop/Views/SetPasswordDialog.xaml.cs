using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using VisualCrypt.Cryptography.Net.Tools;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.PrismSupport;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.Views
{
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public sealed partial class SetPasswordDialog
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

			PwBox.Focus();

			PreviewKeyDown += CloseWithEscape;
			SetMode(paramsProvider.GetParams<SetPasswordDialog, SetPasswordDialogMode>());
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
				var setPasswordResponse = _encryptionService.SetPassword(Encoding.Unicode.GetBytes(PwBox.Text));
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

		void ExecuteClearPasswordCommand()
		{
			PwBox.Text = string.Empty;

		}

		bool CanExecuteClearPasswordCommand()
		{
			if (PwBox.Text.Length > 0)
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
			PwBox.Text = GenerateRandomPassword();
		}

		string GenerateRandomPassword()
		{
			var passwordBytes = new byte[32];

			using (var rng = new RNGCryptoServiceProvider())
				rng.GetBytes(passwordBytes);

			char[] passwordChars = Base64Encoder.EncodeDataToBase64CharArray(passwordBytes);

			string passwordString = new string(passwordChars).Remove(43).Replace("/", "$");
			var sb = new StringBuilder();

			for (var i = 0; i != passwordString.Length; ++i)
			{
				sb.Append(passwordString[i]);
				var insertSpace = (i + 1) % 5 == 0;
				var insertNewLine = (i + 1) % 25 == 0;
				if (insertNewLine)
					sb.Append(Environment.NewLine);
				else if (insertSpace)
					sb.Append(" ");
			}
			return sb.ToString();
		}
	}
}