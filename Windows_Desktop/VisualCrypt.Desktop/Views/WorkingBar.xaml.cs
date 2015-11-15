using System.Windows;
using System.Windows.Controls;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.Desktop.Views
{
    /// <summary>
    /// Interaction logic for Working.xaml
    /// </summary>
    public partial class WorkingBar : UserControl
    {
        public WorkingBar()
        {
            InitializeComponent();
        }

        PortableMainViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as PortableMainViewModel; }
        }

        void WorkingBar_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelLongRunningOperation();
        }
    }
}