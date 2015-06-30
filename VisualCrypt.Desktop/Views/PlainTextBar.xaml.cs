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

		void TextBlock_ClearPassword_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (ViewModel.ClearPasswordCommand.CanExecute())
				ViewModel.ClearPasswordCommand.Execute();
		}

		void Image_IsDirty_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (ViewModel.SaveCommand.CanExecute())
				ViewModel.SaveCommand.CanExecute();
		}
	}
}
