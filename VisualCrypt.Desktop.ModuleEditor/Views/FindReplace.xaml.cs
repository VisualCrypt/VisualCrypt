using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class FindReplace
	{
		[ImportingConstructor]
		public FindReplace(FindReplaceViewModel findReplaceViewModel)
		{
			InitializeComponent();
			DataContext = findReplaceViewModel;

			findReplaceViewModel.MessageBoxService = new MessageBoxService(this);

			if (findReplaceViewModel.TabControlSelectedIndex == 0)
			{
				SetWindowHeight(0);
				TextBoxFindFindString.Focus();
			}
			else
			{
				SetWindowHeight(1);
				TextBoxReplaceFindString.Focus();
			}

			PreviewKeyDown += CloseWithEscape;
			findReplaceViewModel.PropertyChanged += findReplaceViewModel_PropertyChanged;
		}

		void findReplaceViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "TabControlSelectedIndex")
			{
				SetWindowHeight(((FindReplaceViewModel) DataContext).TabControlSelectedIndex);
			}
		}

		void SetWindowHeight(int selectedIndex)
		{
			Height = selectedIndex == 0 ? 183 : 236;
			if (selectedIndex == 0)
				TextBoxFindFindString.Focus();
			else
			{
				TextBoxReplaceString.Focus();
			}
		}

		void CloseWithEscape(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();

			if (e.Key == Key.F && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			{
				((FindReplaceViewModel) DataContext).TabControlSelectedIndex = 0;
			}

			if (e.Key == Key.H && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			{
				((FindReplaceViewModel) DataContext).TabControlSelectedIndex = 1;
			}
		}
	}
}