using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Animation;
using VisualCrypt.Windows.Infrastructure;
using VisualCrypt.Windows.Models;
using VisualCrypt.Windows.Temp;

namespace VisualCrypt.Windows.Pages
{
    class FilesPageViewModel : ViewModelBase, IActiveCleanup
    {
        ObservableCollection<FileReference> _fileReferences;
        IFrameNavigation _frameNavigation;

        public FilesPageViewModel(IFrameNavigation frameNavigation)
        {
            _fileReferences = new ObservableCollection<FileReference>();
            _frameNavigation = frameNavigation;
        }

        public ObservableCollection<FileReference> FileReferences => _fileReferences;

        public DelegateCommand NavigateToNewCommand => CreateCommand(ref _navigateToNewCommand, ExecuteNavigateToNewCommand, () => true);
        DelegateCommand _navigateToNewCommand;

        void ExecuteNavigateToNewCommand()
        {
            _frameNavigation.Frame.Navigate(typeof(MainPage), new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.New }, new DrillInNavigationTransitionInfo());
            Cleanup();
        }

        public DelegateCommand<FileReference> NavigateToOpenCommand => CreateCommand(ref _navigateToOpenCommand, ExecuteNavigateToOpenCommand, arg => true);
        DelegateCommand<FileReference> _navigateToOpenCommand;


        void ExecuteNavigateToOpenCommand(FileReference fileReference)
        {
            _frameNavigation.Frame.Navigate(typeof(MainPage), new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.Open, FileReference = fileReference }, new DrillInNavigationTransitionInfo());
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
            _frameNavigation = null; // release connection to view
        }
    }
}
