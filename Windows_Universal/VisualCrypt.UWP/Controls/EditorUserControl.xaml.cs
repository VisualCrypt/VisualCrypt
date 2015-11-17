using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.UWP.Controls
{
    public sealed partial class EditorUserControl { 

        // public for binding the Print AppBarButton
        public readonly PortableEditorViewModel EditorViewModel;
        public readonly PortableMainViewModel _mainViewModel;
        public EditorUserControl()
        {
            InitializeComponent();
            Service.Get<ITextBoxController>(TextBoxName.TextBox1).PlatformTextBox = TextBox1;
            EditorViewModel = Service.Get<PortableEditorViewModel>();
            _mainViewModel = Service.Get<PortableMainViewModel>();
            Loaded += (s, e) => EditorViewModel.OnViewLoaded();
        }

      

       
    }
}
