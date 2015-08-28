using System.Windows;
using System.Windows.Controls;
using VisualCrypt.Applications.ViewModels;

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

        PortableMainViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as PortableMainViewModel; }
        }

        async void Hyperlink_SetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ShowSetPasswordDialogCommand.CanExecute())
                await ViewModel.ShowSetPasswordDialogCommand.Execute();
        }

        async void Hyperlink_ClearPassword_MouseDown(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ClearPasswordCommand.CanExecute())
                await ViewModel.ClearPasswordCommand.Execute();
        }

        async void Hyperlink_Encrypt_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.EncryptEditorContentsCommand.CanExecute())
                await ViewModel.EncryptEditorContentsCommand.Execute();
        }

        async void Hyperlink_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.DecryptEditorContentsCommand.CanExecute())
                await ViewModel.DecryptEditorContentsCommand.Execute();
        }

        async void Hyperlink_Save_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SaveCommand.CanExecute())
                await ViewModel.SaveCommand.Execute();
        }
    }
}