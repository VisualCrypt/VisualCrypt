using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Cryptography.Portable.Apps.MVVM;
using VisualCrypt.Cryptography.Portable.Apps.Services;
using VisualCrypt.Cryptography.Portable.Apps.Settings;
using VisualCrypt.Cryptography.Portable.Apps.ViewModels;
using VisualCrypt.Windows.Cryptography;
using VisualCrypt.Windows.Services;
using VisualCrypt.Windows.Temp;

namespace VisualCrypt.Windows.Pages
{
    class FilesPageViewModel : ViewModelBase, IActiveCleanup
    {
        ObservableCollection<FileReference> _fileReferences;
        INavigationService _navigationService;

        public FilesPageViewModel(NavigationService navigationService, PasswordDialogDispatcher passwordDialogDispatcher, EncryptionService encryptionService, MessageBoxService messageBoxService, IEventAggregator eventAggregator, ISettingsManager settingsManager, FileService fileService)
        {
            _fileReferences = new ObservableCollection<FileReference>();
            _navigationService = navigationService;
        }
       

        public ObservableCollection<FileReference> FileReferences => _fileReferences;

        public DelegateCommand NavigateToNewCommand => CreateCommand(ref _navigateToNewCommand, ExecuteNavigateToNewCommand, () => true);
        DelegateCommand _navigateToNewCommand;

        void ExecuteNavigateToNewCommand()
        {
            _navigationService.NavigateToMainPage( new FilesPageCommandArgs { FilesPageCommand = FilesPageCommand.New });
            Cleanup();
        }

        public DelegateCommand<FileReference> NavigateToOpenCommand => CreateCommand(ref _navigateToOpenCommand, ExecuteNavigateToOpenCommand, arg => true);
        DelegateCommand<FileReference> _navigateToOpenCommand;

      


        void ExecuteNavigateToOpenCommand(FileReference fileReference)
        {
            _navigationService.NavigateToMainPage(new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.Open, FileReference = fileReference });
            Cleanup();
        }

        

        // Called From FilesPage.OnLoaded
        internal async Task OnNavigatedToCompleteAndLoaded()
        {
            await SampleFiles.CreateSampleFiles();
            // because we await here, OnLoaded can finish - good!
            var fileReferences = await GetFileReferences(ApplicationData.Current.LocalFolder);

            foreach (var fileReference in fileReferences)
            {
                FileReferences.Add(fileReference);
            }
        }

        async Task<ObservableCollection<FileReference>> GetFileReferences(StorageFolder folder)
        {
            StorageFolder fold = folder;

            var items = await fold.GetItemsAsync();
            var files = new ObservableCollection<FileReference>();
            foreach (var item in items)
            {
                if (item is StorageFile)
                    files.Add(new FileReference() { Filename = item.Name, DirectoryName = item.Path });
            }

            return files;
        }

        public void Cleanup()
        {
            _fileReferences = null;  // will also cease fireing SelectionChanged event
            _navigationService = null; // release connection to view
        }
    }
}
