using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Prism.Commands;
using Prism.Events;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Windows.Services;
using VisualCrypt.Windows.Temp;

namespace VisualCrypt.Windows.Pages
{
    class FilesPageViewModel : ViewModelBase, IActiveCleanup
    {
        ObservableCollection<FileReference> _fileReferences;
        public INavigationService NavigationService;
        IMessageBoxService _messageBoxService;
        IFileService _fileService;
        ISettingsManager _settingsManager;
        IEventAggregator _eventAggregator;

        public FilesPageViewModel()
        {
            _fileReferences = new ObservableCollection<FileReference>();
            NavigationService = Service.Get<INavigationService>();
            _messageBoxService = Service.Get<IMessageBoxService>();
            _fileService = Service.Get<IFileService>();
            _settingsManager = Service.Get<ISettingsManager>();
            _eventAggregator = Service.Get<IEventAggregator>();
        }
       

        public ObservableCollection<FileReference> FileReferences => _fileReferences;

        public DelegateCommand NavigateToNewCommand => CreateCommand(ref _navigateToNewCommand, ExecuteNavigateToNewCommand, () => true);
        DelegateCommand _navigateToNewCommand;

        void ExecuteNavigateToNewCommand()
        {
            NavigationService.NavigateToMainPage( new FilesPageCommandArgs { FilesPageCommand = FilesPageCommand.New });
            Cleanup();
        }

        public DelegateCommand<FileReference> NavigateToOpenCommand => CreateCommand(ref _navigateToOpenCommand, ExecuteNavigateToOpenCommand, arg => true);
        DelegateCommand<FileReference> _navigateToOpenCommand;

      


        void ExecuteNavigateToOpenCommand(FileReference fileReference)
        {
            NavigationService.NavigateToMainPage(new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.Open, FileReference = fileReference });
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
                    files.Add(new FileReference() { ShortFilename = item.Name, Filename = item.Path });
            }

            return files;
        }

        public void Cleanup()
        {
            _fileReferences = null;  // will also cease fireing SelectionChanged event
            NavigationService = null; // release connection to view
        }
    }
}
