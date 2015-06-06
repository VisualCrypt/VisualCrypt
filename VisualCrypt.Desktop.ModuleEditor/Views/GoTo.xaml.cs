using System.ComponentModel.Composition;
using System.Windows.Input;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class GoTo
	{
		[ImportingConstructor]
		public GoTo(GoToViewModel goToWindowViewModel)
		{
			InitializeComponent();

			DataContext = goToWindowViewModel;

			TextBoxLineNo.Focus();

			PreviewKeyDown += CloseWithEscape;

			Activated += (sender, args) => TextBoxLineNo.SelectAll();

			Loaded += (sender, args) =>
			{
				goToWindowViewModel.MessageBoxService = new MessageBoxService(this);
			};

			goToWindowViewModel.CloseAction = Close;
		}

		void CloseWithEscape(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}

		
	}
}