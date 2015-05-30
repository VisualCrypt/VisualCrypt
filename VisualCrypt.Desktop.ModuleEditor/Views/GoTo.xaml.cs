using System.Windows.Input;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	public partial class GoTo
	{
		public GoTo(GoToViewModel goToWindowViewModel)
		{
			InitializeComponent();

			DataContext = goToWindowViewModel;

			TextBoxLineNo.Focus();

			PreviewKeyDown += CloseWithEscape;

			goToWindowViewModel.CloseAction = Close;
		}

		void CloseWithEscape(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}

		public void SelectAll()
		{
			TextBoxLineNo.SelectAll();
		}
	}
}