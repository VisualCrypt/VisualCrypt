using System.ComponentModel;
using System.Windows.Input;

namespace VisualCrypt.Desktop.ModuleEditor.Views
{
    public partial class FindReplace
    {
        public FindReplace(FindReplaceViewModel findReplaceViewModel)
        {
            InitializeComponent();
            DataContext = findReplaceViewModel;

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
                SetWindowHeight(((FindReplaceViewModel)DataContext).TabControlSelectedIndex);
            }
        }

        void SetWindowHeight(int selectedIndex)
        {
            Height = selectedIndex == 0 ? 183 : 236;
            if(selectedIndex == 0)
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
                ((FindReplaceViewModel)DataContext).TabControlSelectedIndex = 0;
               
                
            }

            if (e.Key == Key.H && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                ((FindReplaceViewModel)DataContext).TabControlSelectedIndex = 1;
               
            }

        }

    }
}