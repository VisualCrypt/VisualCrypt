using VisualCrypt.Cryptography.Portable.Apps.Models;

namespace VisualCrypt.Cryptography.Portable.Apps.Services
{
    public  interface INavigationService
    {
        void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs);
        void NavigateToFilesPage();
    }
}