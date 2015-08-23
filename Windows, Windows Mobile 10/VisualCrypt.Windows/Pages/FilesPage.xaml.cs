using System.Linq;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Portable.Apps.Models;
using VisualCrypt.Windows.Cryptography;
using VisualCrypt.Windows.Services;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class FilesPage
    {
        readonly FilesPageViewModel _viewModel;

        public  FilesPage()
        {
            InitializeComponent();
            _viewModel = new FilesPageViewModel(new NavigationService(Frame),
                new PasswordDialogDispatcher(), new EncryptionService(), new MessageBoxService(), Svc.EventAggregator,
                Svc.SettingsManager, new FileService());
            Loaded += async (s,e)=> await _viewModel.OnNavigatedToCompleteAndLoaded();
        }

       

        void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileReference = e.AddedItems.FirstOrDefault() as FileReference;
            _viewModel.NavigateToOpenCommand.Execute(fileReference);
        }
    }
}
