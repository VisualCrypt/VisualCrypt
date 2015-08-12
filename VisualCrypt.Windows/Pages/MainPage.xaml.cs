using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Cryptography.Portable.Apps.Models;
using VisualCrypt.Cryptography.Portable.Apps.ViewModels;
using VisualCrypt.Windows.Services;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPage
    {
        readonly PortableMainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new PortableMainViewModel(Svc.EventAggregator, Svc.Log,
                                                        Svc.MessageBoxService, Svc.EncryptionService,
                                                        Svc.NavigationService(Frame), Svc.PasswordDialogDispatcher,
                                                        Svc.SettingsManager, Svc.FileService,
                                                        Svc.BrowserService, Svc.AssemblyInfoProvider,
                                                        Svc.LifeTimeService, Svc.ClipBoardService,
                                                        Svc.WindowManager);
            EditorControl.Context = _viewModel;
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
