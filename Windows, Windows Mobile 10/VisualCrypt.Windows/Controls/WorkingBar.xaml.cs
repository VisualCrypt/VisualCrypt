using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class WorkingBar
    {
        readonly PortableMainViewModel _viewModel;

        public WorkingBar()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
        }

        void WorkingBar_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CancelLongRunningOperation();
        }
    }
}
