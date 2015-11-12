using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class HelpPage : Page
    {
        IAssemblyInfoProvider _aip;
        ResourceWrapper _resourceWrapper;
        string _title;

        public HelpPage()
        {
            InitializeComponent();
            _aip = Service.Get<IAssemblyInfoProvider>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _title = _resourceWrapper.miHelpAbout.NoDots();
        }

        string Title => _title;
        IAssemblyInfoProvider AIP => _aip;
        ResourceWrapper ResourceWrapper => _resourceWrapper;
       
        void AppBarButton_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FilesPage));
        }

    }
}
