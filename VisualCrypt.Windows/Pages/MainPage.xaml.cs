using System.Linq.Expressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Windows.Controls.EditorSupport;
using VisualCrypt.Windows.Services;
using VisualCrypt.Windows.Static;
using VisualCrypt.Windows.V2;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPage
    {
        readonly MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel(new NavigationService(Frame),
                new PasswordDialogDispatcher(), new EncryptionService(), new MessageBoxService(), SharedInstances.EventAggregator,
                SharedInstances.SettingsManager, new FileService()
                 );
            this.EditorControl.Context = (IEditorContext) _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RoutedEventHandler handler = null;
            handler = async (sender, args) => 
            {
                Loaded -= handler;
                await _viewModel.OnNavigatedToCompletedAndLoaded((FilesPageCommandArgs) e.Parameter);
            };
            Loaded += handler;
        }

        
    }
}
