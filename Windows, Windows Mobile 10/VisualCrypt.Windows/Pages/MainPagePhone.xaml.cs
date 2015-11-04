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
    public sealed partial class MainPagePhone
    {
        readonly PortableMainViewModel _viewModel;
        readonly SettingsManager _settingsManager;

        public MainPagePhone()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
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
