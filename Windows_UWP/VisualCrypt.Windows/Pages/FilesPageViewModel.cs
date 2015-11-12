using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Prism.Commands;
using Prism.Events;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Windows.Services;
using VisualCrypt.Applications.Services.PortableImplementations;
using System.Collections.Generic;
using VisualCrypt.Windows.Controls;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Windows.Pages
{
    class FilesPageViewModel : ViewModelBase, IActiveCleanup
    {
        ObservableCollection<FileReference> _fileReferences;
        public List<FileReference> MultiSelectedFileReferences;
        public FileReference SingleSelectedFileReference;
        public int SelectedItemsCount;

        public INavigationService NavigationService;
        IMessageBoxService _messageBoxService;
        IFileService _fileService;
        SettingsManager _settingsManager;
        IEventAggregator _eventAggregator;
        ResourceWrapper _resourceWrapper;

        public FilesPageViewModel()
        {
            _fileReferences = new ObservableCollection<FileReference>();
            NavigationService = Service.Get<INavigationService>();
            _messageBoxService = Service.Get<IMessageBoxService>();
            _fileService = Service.Get<IFileService>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _eventAggregator = Service.Get<IEventAggregator>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            var dummy = ResourceWrapper.uriSpecUrl;
        }


        public ObservableCollection<FileReference> FileReferences => _fileReferences;
        public ResourceWrapper ResourceWrapper => _resourceWrapper;

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isEditMode;

        public DelegateCommand NavigateToHelpCommand => CreateCommand(ref _navigateToHelpCommand, ExecuteNavigateToHelpCommand, () => true);
        DelegateCommand _navigateToHelpCommand;
        void ExecuteNavigateToHelpCommand()
        {
            NavigationService.NavigateToHelpPage();
            Cleanup();
        }

        public DelegateCommand NavigateToSettingsCommand => CreateCommand(ref _navigateToSettingsCommand, ExecuteNavigateToSettingsCommand, () => true);
        DelegateCommand _navigateToSettingsCommand;
        void ExecuteNavigateToSettingsCommand()
        {
            NavigationService.NavigateToSettingsPage();
            Cleanup();
        }

        public DelegateCommand NavigateToNewCommand => CreateCommand(ref _navigateToNewCommand, ExecuteNavigateToNewCommand, () => true);
        DelegateCommand _navigateToNewCommand;

        void ExecuteNavigateToNewCommand()
        {
            NavigationService.NavigateToMainPage(new FilesPageCommandArgs { FilesPageCommand = FilesPageCommand.New });
            Cleanup();
        }

        public DelegateCommand<FileReference> NavigateToOpenCommand => CreateCommand(ref _navigateToOpenCommand, ExecuteNavigateToOpenCommand, arg => true);
        DelegateCommand<FileReference> _navigateToOpenCommand;




        void ExecuteNavigateToOpenCommand(FileReference fileReference)
        {
            NavigationService.NavigateToMainPage(new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.Open, FileReference = fileReference });
            Cleanup();
        }

        public DelegateCommand SelectCommand => CreateCommand(ref _selectCommand, ExecuteSelectCommand, () => FileReferences.Count > 0);
        DelegateCommand _selectCommand;

        void ExecuteSelectCommand()
        {
            IsEditMode = true;
        }

        public DelegateCommand CancelSelectCommand => CreateCommand(ref _cancelSelectCommand, ExecuteCancelSelectCommand, () => true);
        DelegateCommand _cancelSelectCommand;

        void ExecuteCancelSelectCommand()
        {
            IsEditMode = false;
        }

        public DelegateCommand DeleteCommand => CreateCommand(ref _deleteCommand, ExecuteDeleteCommand, () => SelectedItemsCount > 0);
        DelegateCommand _deleteCommand;
        async void ExecuteDeleteCommand()
        {
            if (SelectedItemsCount == 1)
            {
                var result = await _messageBoxService.Show("Delete 1 File?", "Delete", RequestButton.OKCancel, RequestImage.Question);
                var fileToDelete = (StorageFile)SingleSelectedFileReference.FileSystemObject;
                await fileToDelete.DeleteAsync();
            }
            else
            {
                var result = await _messageBoxService.Show(string.Format("Delete {0} Files?", SelectedItemsCount), "Delete", RequestButton.OKCancel, RequestImage.Question);
                var filesToDelete = MultiSelectedFileReferences;
                foreach (var file in MultiSelectedFileReferences)
                {
                    var fileToDelete = (StorageFile)file.FileSystemObject;
                    await fileToDelete.DeleteAsync();
                }
            }
            await OnNavigatedToCompleteAndLoaded();
            await CancelSelectCommand.Execute();
        }

        public DelegateCommand RenameCommand => CreateCommand(ref _renameCommand, ExecuteRenameCommand, () => SelectedItemsCount == 1);
        DelegateCommand _renameCommand;
        async void ExecuteRenameCommand()
        {
            try
            {
                var storageFile = (StorageFile)SingleSelectedFileReference.FileSystemObject;
                var result = await _fileService.PickFileAsync(storageFile.Path, DialogFilter.VisualCrypt, FileDialogMode.Rename, null);
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e.Message);
            }
            await OnNavigatedToCompleteAndLoaded();
            await CancelSelectCommand.Execute();
        }

        // Called From FilesPage.OnLoaded and when navigating back from MainPage
        internal async Task OnNavigatedToCompleteAndLoaded()
        {
            //await SampleFiles.CreateSampleFiles();

            FileReferences.Clear();
            var fileReferences = await GetFileReferences(ApplicationData.Current.LocalFolder);

            foreach (var fileReference in fileReferences)
            {
                FileReferences.Add(fileReference);
            }
            SelectCommand.RaiseCanExecuteChanged();
        }

        async Task<ObservableCollection<FileReference>> GetFileReferences(StorageFolder folder)
        {
            StorageFolder fold = folder;

            var items = await fold.GetItemsAsync();
            var files = new ObservableCollection<FileReference>();
            foreach (var item in items)
            {
                var storageFile = item as StorageFile;
                if (storageFile == null)
                    continue;
                if (storageFile.FileType.ToLowerInvariant() != ".visualcrypt")
                    continue;
                var basicProperties = await storageFile.GetBasicPropertiesAsync();
                var modifiedDate = basicProperties.DateModified.ToLocalTime().ToString();
                files.Add(new FileReference()
                {
                    ShortFilename = storageFile.Name,
                    PathAndFileName = storageFile.Path,
                    ModifiedDate = modifiedDate,
                    FileSystemObject = storageFile
                });
            }

            return files;
        }

        public void Cleanup()
        {
            //_fileReferences = null;  // will also cease fireing SelectionChanged event
            // NavigationService = null; // release connection to view
        }
    }
}
