using System.Windows;
using System.Windows.Controls;

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

        ShellViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ShellViewModel; }
        }

        void WorkingBar_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelLongRunningOperation();
        }
    }
}