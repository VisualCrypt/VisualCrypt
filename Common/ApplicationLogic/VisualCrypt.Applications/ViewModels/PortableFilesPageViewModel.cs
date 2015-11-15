using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prism.Commands;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Applications.ViewModels
{
    public class PortableFilesPageViewModel : ViewModelBase, IActiveCleanup
    {
        public List<FileReference> MultiSelectedFileReferences;
        public FileReference SingleSelectedFileReference;
        public int SelectedItemsCount;

        readonly INavigationService _navigationService;
        readonly IMessageBoxService _messageBoxService;
        readonly IFileService _fileService;
        readonly AbstractSettingsManager _settingsManager;
        readonly IEncryptionService _encryptionService;
        readonly ILifeTimeService _lifeTimeService;

        public PortableFilesPageViewModel()
        {
            FileReferences = new ObservableCollection<FileReference>();
            ResourceWrapper = Service.Get<ResourceWrapper>();

            _navigationService = Service.Get<INavigationService>();
            _messageBoxService = Service.Get<IMessageBoxService>();
            _fileService = Service.Get<IFileService>();
            _settingsManager = Service.Get<AbstractSettingsManager>();
            _encryptionService = Service.Get<IEncryptionService>();
            _lifeTimeService = Service.Get<ILifeTimeService>();
        }

        public ObservableCollection<FileReference> FileReferences { get; }
        public ResourceWrapper ResourceWrapper { get; }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            private set
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
            _navigationService.NavigateToHelpPage();
            Cleanup();
        }

        public DelegateCommand NavigateToSettingsCommand => CreateCommand(ref _navigateToSettingsCommand, ExecuteNavigateToSettingsCommand, () => true);
        DelegateCommand _navigateToSettingsCommand;
        void ExecuteNavigateToSettingsCommand()
        {
            _navigationService.NavigateToSettingsPage();
            Cleanup();
        }

        public DelegateCommand NavigateToNewCommand => CreateCommand(ref _navigateToNewCommand, ExecuteNavigateToNewCommand, () => true);
        DelegateCommand _navigateToNewCommand;

        void ExecuteNavigateToNewCommand()
        {
            _navigationService.NavigateToMainPage(new FilesPageCommandArgs { FilesPageCommand = FilesPageCommand.New });
            Cleanup();
        }

        // ClearPasswordAndExitCommand
        public DelegateCommand ClearPasswordAndExitCommand => CreateCommand(ref _clearPasswordAndExitCommand, ExecuteClearPasswordAndExitCommand, () => true);
        DelegateCommand _clearPasswordAndExitCommand;

        async void ExecuteClearPasswordAndExitCommand()
        {
            var isPasswordSet = Service.Get<PortableMainViewModel>().PasswordInfo.IsPasswordSet;
            if (!isPasswordSet)
            {
                Cleanup();
                _lifeTimeService.HandleExitRequested(null, null);
                return;
            }

            var result = await _messageBoxService.Show("Clear Session Password Hash and Exit?", "", RequestButton.OKCancel,
                RequestImage.Question);
            if (result == RequestResult.OK)
            {
                var response = _encryptionService.ClearPassword();
                if (response.IsSuccess)
                {
                    Cleanup();
                    _lifeTimeService.HandleExitRequested(null, null);
                }
                else
                {
                    await _messageBoxService.ShowError(response.Error);
                }
            }

        }


        public DelegateCommand<FileReference> NavigateToOpenCommand => CreateCommand(ref _navigateToOpenCommand, ExecuteNavigateToOpenCommand, arg => true);
        DelegateCommand<FileReference> _navigateToOpenCommand;
        void ExecuteNavigateToOpenCommand(FileReference fileReference)
        {
            _navigationService.NavigateToMainPage(new FilesPageCommandArgs() { FilesPageCommand = FilesPageCommand.Open, FileReference = fileReference });
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
            SelectCommand.RaiseCanExecuteChanged();
        }

        public DelegateCommand DeleteCommand => CreateCommand(ref _deleteCommand, ExecuteDeleteCommand, () => SelectedItemsCount > 0);
        DelegateCommand _deleteCommand;
        async void ExecuteDeleteCommand()
        {
            if (SelectedItemsCount == 1)
            {
                var result = await _messageBoxService.Show("Delete 1 File?", "Delete", RequestButton.OKCancel, RequestImage.Question);
                if (result == RequestResult.OK)
                {
                    await _fileService.DeleteAsync(SingleSelectedFileReference.PathAndFileName);
                }
            }
            else
            {
                var result = await _messageBoxService.Show(string.Format("Delete {0} Files?", SelectedItemsCount), "Delete", RequestButton.OKCancel, RequestImage.Question);
                if (result == RequestResult.OK)
                {
                    foreach (var file in MultiSelectedFileReferences)
                    {
                        await _fileService.DeleteAsync(file.PathAndFileName);
                    }
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
                // PortableFilenameDialogViewModel handles all the action, therefore the result can be ignored.
                await _fileService.PickFileAsync(SingleSelectedFileReference.PathAndFileName, DialogFilter.VisualCrypt, FileDialogMode.Rename, null);
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e.Message);
            }
            await OnNavigatedToCompleteAndLoaded();
            await CancelSelectCommand.Execute();
        }

        public async Task OnNavigatedToCompleteAndLoaded()
        {
            FileReferences.Clear();
            var fileReferences = await _fileService.GetFileReferences(_settingsManager.CurrentDirectoryName);

            foreach (var fileReference in fileReferences)
            {
                FileReferences.Add(fileReference);
            }
            SelectCommand.RaiseCanExecuteChanged();
        }

        public void Cleanup()
        {

        }
    }
}
