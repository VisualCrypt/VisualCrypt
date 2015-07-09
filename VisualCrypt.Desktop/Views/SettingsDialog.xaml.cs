using System.Runtime.CompilerServices;
using Microsoft.Practices.Prism.Commands;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.Views
{
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public sealed partial class SettingsDialog : INotifyPropertyChanged
	{
		readonly IMessageBoxService _messageBoxService;
		readonly IEncryptionService _encryptionService;

		[ImportingConstructor]
		public SettingsDialog(IMessageBoxService messageBoxService,
			IEncryptionService encryptionService)
		{
			_messageBoxService = messageBoxService;
			_encryptionService = encryptionService;

			InitializeComponent();
			DataContext = this;

		

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
				//var setPasswordResponse = _encryptionService.SetPassword(Encoding.Unicode.GetBytes(PwBox.Text));
				//if (!setPasswordResponse.IsSuccess)
				//{
				//	PasswordManager.PasswordInfo.IsPasswordSet = false;
				//	_messageBoxService.ShowError(setPasswordResponse.Error);
				//	return;
				//}
				//PasswordManager.PasswordInfo.IsPasswordSet = true;
				DialogResult = true;
				Close();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
			finally
			{
				

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
			

		}

		bool CanExecuteClearPasswordCommand()
		{
			if (1 > 0)
			{
				return true;
			}
			
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

		public int LogRounds
		{
			get { return _logRounds; }
			set
			{
				if (_logRounds != value)
				{
					_logRounds = value;
					OnPropertyChanged();
				}
			}
		}

		int _logRounds;

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}