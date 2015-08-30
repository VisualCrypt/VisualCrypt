using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Windows.Services;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Services.PortableImplementations;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPage
    {
        readonly PortableMainViewModel _viewModel;
        readonly SettingsManager _settingsManager;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            TopBar.Opening += TopBarOpened;
            TopBar.Closing += TopBarClosed;
            TopBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
            TopBar.IsSticky = true;
            
        }

        private void TopBarClosed(object sender, object e)
        {
            this.TopBarSpacer.Height =0;
        }

        private void TopBarOpened(object sender, object e)
        {
            this.TopBarSpacer.Height = 12;
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
