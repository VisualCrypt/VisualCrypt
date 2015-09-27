using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Desktop.Views
{
    
    public partial class AboutDialog
    {
        IAssemblyInfoProvider _aip;
        ResourceWrapper _resourceWrapper;
        public AboutDialog()
        {
            InitializeComponent();
            DataContext = this;
            PreviewKeyDown += CloseWithEscape;
            _aip = Service.Get<IAssemblyInfoProvider>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            Title = _resourceWrapper.miHelpAbout.NoDots();
        }

        void CloseWithEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public IAssemblyInfoProvider  AIP
        {
            get { return _aip; }
        }

        public ResourceWrapper ResourceWrapper
        {
            get { return _resourceWrapper; }
        }


        void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void Hyperlink_License_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}