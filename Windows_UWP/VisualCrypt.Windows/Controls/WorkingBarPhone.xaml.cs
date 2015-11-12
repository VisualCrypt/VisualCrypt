using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class WorkingBarPhone
    {
        readonly PortableMainViewModel _viewModel;

        public WorkingBarPhone()
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
