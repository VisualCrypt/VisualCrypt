using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Prism.Commands;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.Services;
using VisualCrypt.Language;

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

			LogRounds = SettingsManager.EditorSettings.CryptographySettings.LogRounds;


			PreviewKeyDown += CloseWithEscape;
		}


		void CloseWithEscape(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}


		public DelegateCommand SaveCommand
		{
			get
			{
				if (_saveCommand != null)
					return _saveCommand;
				_saveCommand = new DelegateCommand(ExecuteSaveCommand, () => true);
				return _saveCommand;
			}
		}

		DelegateCommand _saveCommand;

		void ExecuteSaveCommand()
		{
			try
			{
				SettingsManager.EditorSettings.CryptographySettings.LogRounds = LogRounds;
				DialogResult = true;
				Close();
			}
			catch (Exception e)
			{
				_messageBoxService.ShowError(e);
			}
		}


		DelegateCommand _defaultsCommand;

		public DelegateCommand DefaultsCommand
		{
			get
			{
				if (_defaultsCommand != null)
					return _defaultsCommand;
				_defaultsCommand = new DelegateCommand(ExecuteDefaultsCommand, CanExecuteDefaultsCommand);
				return _defaultsCommand;
			}
		}

		void ExecuteDefaultsCommand()
		{
			LogRounds = BCrypt.DefaultBCryptRoundsLog2;
		}

		bool CanExecuteDefaultsCommand()
		{
			return LogRounds != BCrypt.DefaultBCryptRoundsLog2;
		}

		void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}


		void Hyperlink_Spec_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var process = new Process {StartInfo = {UseShellExecute = true, FileName = Loc.Strings.uriPWSpecUrl}})
					process.Start();
			}
			catch (Exception ex)
			{
				_messageBoxService.ShowError(ex);
			}
		}

		void Hyperlink_Source_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var process = new Process {StartInfo = {UseShellExecute = true, FileName = Loc.Strings.uriPWSpecUrl } })
					process.Start();
			}
			catch (Exception ex)
			{
				_messageBoxService.ShowError(ex);
			}
		}


		public byte LogRounds
		{
			get { return _logRounds; }
			set
			{
				_logRounds = value;
				OnPropertyChanged();
				SetWarningText(value);
				DefaultsCommand.RaiseCanExecuteChanged();
			}
		}

		byte _logRounds;

		void SetWarningText(byte logRounds)
		{
			if (logRounds == BCrypt.DefaultBCryptRoundsLog2)
			{
				TextBlockWarning.Text =
					"The setting influences the required computational work to create the BCrypt hash. A higher value means more work.";
				return;
			}
			TextBlockWarning.Text = logRounds > BCrypt.DefaultBCryptRoundsLog2
				? "Warning: A high value will turn encryption and decryption into a very time consuming operation."
				: "Warning: A low value faciliates brute force and dictionary attacks.";
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