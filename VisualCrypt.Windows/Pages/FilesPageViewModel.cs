using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using VisualCrypt.Windows.Infrastructure;
using VisualCrypt.Windows.Models;

namespace VisualCrypt.Windows.Pages
{
    class FilesPageViewModel : ViewModelBase
    {
        public readonly ObservableCollection<FileReference> FileReferences = new ObservableCollection<FileReference>();
        Frame _frame;

        public FilesPageViewModel()
        {
            FileReferences.Add(new FileReference {Filename = "File1.visualcrpyt", DirectoryName = "Desktop"});
            FileReferences.Add(new FileReference { Filename = "File2.visualcrpyt", DirectoryName = "Desktop" });
        }

        public DelegateCommand NavigateToNewCommand => CreateCommand(ref _navigateToNewCommand, ExecuteNavigateToNewCommand, () => true);
        DelegateCommand _navigateToNewCommand;

        void ExecuteNavigateToNewCommand()
        {
            _frame.Navigate(typeof(MainPage), new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.New }, new DrillInNavigationTransitionInfo());
        }

        public DelegateCommand<FileReference> NavigateToOpenCommand => CreateCommand(ref _navigateToOpenCommand, ExecuteNavigateToOpenCommand, arg => true);
        DelegateCommand<FileReference> _navigateToOpenCommand;

        void ExecuteNavigateToOpenCommand(FileReference fileReference)
        {
            _frame.Navigate(typeof(MainPage), new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.Open, FileReference = fileReference }, new DrillInNavigationTransitionInfo());
        }

        internal async void SetNavigationService(Frame frame)
        {
            _frame = frame;
           
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

    }
}
