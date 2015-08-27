using System.Linq;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications;
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
            //_viewModel.NavigationService.Context = Frame;
            Loaded += async (s,e)=> await _viewModel.OnNavigatedToCompleteAndLoaded();
        }

       

        void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var fileReference = e.AddedItems.FirstOrDefault() as FileReference;
            //_viewModel.NavigateToOpenCommand.Execute(fileReference);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var fileReference = e.ClickedItem as FileReference;
            _viewModel.NavigateToOpenCommand.Execute(fileReference);
        }
    }
}
