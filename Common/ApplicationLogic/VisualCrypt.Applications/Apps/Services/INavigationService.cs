using VisualCrypt.Applications.Apps.Models;

namespace VisualCrypt.Applications.Apps.Services
{
    public  interface INavigationService
    {
        void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs);
        void NavigateToFilesPage();
    }
}