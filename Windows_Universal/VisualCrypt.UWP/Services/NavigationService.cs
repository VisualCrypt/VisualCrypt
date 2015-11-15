using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.UWP.Pages;

namespace VisualCrypt.UWP.Services
{
    class NavigationService : INavigationService
    {
        // Guidelines for page transition animations
        // https://msdn.microsoft.com/en-us/library/windows/apps/jj635239.aspx

        Frame CurrentFrame => Window.Current.Content as Frame ?? new Frame();

        public void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs)
        {
            CurrentFrame.Navigate(typeof(MainPagePhone), filesPageCommandArgs, new DrillInNavigationTransitionInfo());
        }

        public void NavigateToFilesPage()
        {
            CurrentFrame.Navigate(typeof(FilesPage), new EntranceNavigationTransitionInfo());
        }

        public void NavigateToHelpPage()
        {
            CurrentFrame.Navigate(typeof(HelpPage), new DrillInNavigationTransitionInfo());
        }

        public void NavigateToSettingsPage()
        {
            CurrentFrame.Navigate(typeof(SettingsPage), new DrillInNavigationTransitionInfo());
        }
    }
}
