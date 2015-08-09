using Windows.UI.Xaml.Controls;

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class Editor : UserControl
    {
        readonly EditorViewModel _viewModel = new EditorViewModel();
        public Editor()
        {
            InitializeComponent();
            Loaded += (s, e) => _viewModel.OnViewInitialized(_textBox1);
        }
    }
}
