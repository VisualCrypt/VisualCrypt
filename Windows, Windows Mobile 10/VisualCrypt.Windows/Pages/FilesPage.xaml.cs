using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class FilesPage
    {
        readonly FilesPageViewModel _viewModel;

        public  FilesPage()
        {
            InitializeComponent();
            _viewModel = Service.Get<FilesPageViewModel>();
            Loaded += async (s,e)=> await _viewModel.OnNavigatedToCompleteAndLoaded();
        }


        async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var fileReference = e.ClickedItem as FileReference;
            await _viewModel.NavigateToOpenCommand.Execute(fileReference);
        }
    }
}
