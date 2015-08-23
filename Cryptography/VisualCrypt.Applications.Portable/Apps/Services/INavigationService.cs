using VisualCrypt.Applications.Portable.Apps.Models;

namespace VisualCrypt.Applications.Portable.Apps.Services
{
    public  interface INavigationService
    {
        void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs);
        void NavigateToFilesPage();
    }
}