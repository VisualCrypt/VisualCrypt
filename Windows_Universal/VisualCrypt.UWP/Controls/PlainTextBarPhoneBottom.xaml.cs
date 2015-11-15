using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.UWP.Controls
{
    public sealed partial class PlainTextBarPhoneBottom
    {
        readonly PortableMainViewModel _viewModel;

        public PlainTextBarPhoneBottom()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
        }
    }
}
