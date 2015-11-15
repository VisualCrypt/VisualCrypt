using Windows.UI.Xaml;
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
        }

        IAssemblyInfoProvider AIP => _aip;
        string Title => _title;
       
        void AppBarButton_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FilesPage));
        }

    }
}
