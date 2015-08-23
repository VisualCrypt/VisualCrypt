using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using VisualCrypt.Cryptography.Portable.Apps.Services;

namespace VisualCrypt.Windows.Pages
{
    class NavigationService : INavigationService
    {
        readonly Frame _frame;
        public NavigationService(Frame frame)
        {
            _frame = frame;
        }
        public void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs)
        {
            _frame.Navigate(typeof(MainPage), filesPageCommandArgs, new DrillInNavigationTransitionInfo());
        }

        public void NavigateToFilesPage()
        {
            _frame.Navigate(typeof(FilesPage),  new DrillInNavigationTransitionInfo());
        }
    }
}
