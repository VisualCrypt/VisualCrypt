using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Windows.Services;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPage
    {
        readonly PortableMainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RoutedEventHandler handler = null;
            handler = async (sender, args) =>
            {
                Loaded -= handler;
                await _viewModel.OnNavigatedToCompletedAndLoaded((FilesPageCommandArgs)e.Parameter);
            };
            Loaded += handler;
        }


    }
}
