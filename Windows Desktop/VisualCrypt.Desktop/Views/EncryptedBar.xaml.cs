using System.Windows;
using System.Windows.Controls;

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

        void Hyperlink_CopyAll_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CopyAllCommand.CanExecute())
                ViewModel.CopyAllCommand.Execute();
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