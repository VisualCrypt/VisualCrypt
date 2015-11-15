using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.UWP.Pages
{
    public sealed partial class HelpPage
    {
        readonly IAssemblyInfoProvider _aip;
        readonly string _title;

        public HelpPage()
        {
            InitializeComponent();
            _aip = Service.Get<IAssemblyInfoProvider>();
            _title = Service.Get<ResourceWrapper>().miHelpAbout.NoDots();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
        }

        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            Frame.Navigate(typeof(FilesPage), new EntranceNavigationTransitionInfo());

        }

        IAssemblyInfoProvider AIP => _aip;
        string Title => _title;
       
        void AppBarButton_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FilesPage), new EntranceNavigationTransitionInfo());
        }

    }
}
