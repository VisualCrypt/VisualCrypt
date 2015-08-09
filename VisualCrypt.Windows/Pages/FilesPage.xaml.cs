using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Windows.Models;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class FilesPage
    {
        readonly FilesPageViewModel _viewModel = new FilesPageViewModel();

        public  FilesPage()
        {
            InitializeComponent();
            //var t= Task.Run(() => Temp.SampleFiles.CreateSampleFiles());
            //t.Wait();
            Loaded += (s,e)=> _viewModel.SetNavigationService(Frame);
        }

       

        void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileReference = e.AddedItems.FirstOrDefault() as FileReference;
            _viewModel.NavigateToOpenCommand.Execute(fileReference);
        }
    }
}
