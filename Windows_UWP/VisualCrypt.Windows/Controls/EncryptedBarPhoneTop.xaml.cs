using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class EncryptedBarPhoneTop : UserControl
    {
        readonly PortableMainViewModel _viewModel;

        public EncryptedBarPhoneTop()
        {
            this.InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
        }


   

        async void Hyperlink_SetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ShowSetPasswordDialogCommand.CanExecute())
                await _viewModel.ShowSetPasswordDialogCommand.Execute();
        }

        async void Hyperlink_ClearPassword_MouseDown(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ClearPasswordCommand.CanExecute())
                await _viewModel.ClearPasswordCommand.Execute();
        }

        async void Hyperlink_CopyAll_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CopyAllCommand.CanExecute())
                await _viewModel.CopyAllCommand.Execute();
        }

        async void Hyperlink_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DecryptEditorContentsCommand.CanExecute())
                await _viewModel.DecryptEditorContentsCommand.Execute();
        }

        async void Hyperlink_Save_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SaveCommand.CanExecute())
                await _viewModel.SaveCommand.Execute();
        }
    }
}
