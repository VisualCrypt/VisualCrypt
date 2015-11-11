using VisualCrypt.Applications.Models;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public  interface INavigationService
    {
      
        void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs);
        void NavigateToFilesPage();
        void NavigateToHelpPage();
        void NavigateToSettingsPage();
    }
}