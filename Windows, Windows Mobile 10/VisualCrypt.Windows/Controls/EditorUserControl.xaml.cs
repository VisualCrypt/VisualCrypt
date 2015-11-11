using VisualCrypt.Applications;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class EditorUserControl { 

        // public for binding the Print AppBarButton
        public readonly PortableEditorViewModel _viewModel;
        public EditorUserControl()
        {
            InitializeComponent();
            Service.Get<ITextBoxController>(TextBoxName.TextBox1).PlatformTextBox = TextBox1;
            _viewModel = Service.Get<PortableEditorViewModel>();
          
            Loaded += (s, e) => _viewModel.OnViewLoaded();
        }

      

       
    }
}
