using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Windows.Models;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class FilesPage: IFrameNavigation
    {
        readonly FilesPageViewModel _viewModel;

        public  FilesPage()
        {
            InitializeComponent();
            _viewModel = new FilesPageViewModel(this);
            Loaded += async (s,e)=> await _viewModel.OnNavigatedToCompleteAndLoaded();
        }

       

        void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileReference = e.AddedItems.FirstOrDefault() as FileReference;
            _viewModel.NavigateToOpenCommand.Execute(fileReference);
        }
    }
}
