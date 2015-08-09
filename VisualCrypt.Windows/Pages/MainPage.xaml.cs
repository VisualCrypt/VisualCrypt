using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPage : IFrameNavigation
    {
        readonly MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RoutedEventHandler handler = null;
            handler = async (sender, args) => 
            {
                Loaded -= handler;
                await _viewModel.OnNavigatedToCompletedAndLoaded(e);
            };
            Loaded += handler;
        }

        
    }
}
