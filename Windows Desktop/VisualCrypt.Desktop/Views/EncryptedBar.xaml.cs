using System.Windows;
using System.Windows.Controls;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Desktop.Views
{
    /// <summary>
    /// Interaction logic for EncryptedBar.xaml
    /// </summary>
    public partial class EncryptedBar : UserControl
    {
        public EncryptedBar()
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

        async void Hyperlink_CopyAll_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CopyAllCommand.CanExecute())
                await ViewModel.CopyAllCommand.Execute();
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