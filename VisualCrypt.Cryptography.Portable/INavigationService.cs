namespace VisualCrypt.Cryptography.Portable
{
    public  interface INavigationService
    {
        void NavigateToMainPage(FilesPageCommandArgs filesPageCommandArgs);
        void NavigateToFilesPage();
    }
}