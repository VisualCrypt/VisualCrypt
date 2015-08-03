using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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