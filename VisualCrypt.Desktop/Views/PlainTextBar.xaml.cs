using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisualCrypt.Desktop.Views
{
	/// <summary>
	/// Interaction logic for PlainTextBar.xaml
	/// </summary>
	public partial class PlainTextBar : UserControl
	{
		public PlainTextBar()
		{
			InitializeComponent();
		}

		ShellViewModel ViewModel
		{
			set { DataContext = value; }
			get { return DataContext as ShellViewModel; }
		}

		void Hyperlink_SetPassword_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.ShowSetPasswordDialogCommand.CanExecute())
				ViewModel.ShowSetPasswordDialogCommand.Execute();
		}

		void Hyperlink_ClearPassword_MouseDown(object sender, RoutedEventArgs e)
		{
			if (ViewModel.ClearPasswordCommand.CanExecute())
				ViewModel.ClearPasswordCommand.Execute();
		}

		void Hyperlink_Encrypt_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.EncryptEditorContentsCommand.CanExecute())
				ViewModel.EncryptEditorContentsCommand.Execute();
		}

		void Hyperlink_Decrypt_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.DecryptEditorContentsCommand.CanExecute())
				ViewModel.DecryptEditorContentsCommand.Execute();
		}

		void Hyperlink_Save_Click(object sender, RoutedEventArgs e)
		{
			if (ViewModel.SaveCommand.CanExecute())
				ViewModel.SaveCommand.Execute();
		}
	}
}