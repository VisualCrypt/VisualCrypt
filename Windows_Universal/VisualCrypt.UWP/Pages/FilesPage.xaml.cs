using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Language.Strings;
using VisualCrypt.UWP.Styles;

namespace VisualCrypt.UWP.Pages
{
    /// <summary>
    /// See MainPage for documentation!
    /// </summary>
    sealed partial class FilesPage
    {
        public static FilesPage PageReference { get; private set; }
        static bool _isFirstLoopComplete;

        readonly PortableFilesPageViewModel _viewModel;
        readonly PortableMainViewModel _mainViewModel;
        readonly ResourceWrapper _resourceWrapper;
        readonly IFileService _fileService;
        readonly IMessageBoxService _messageBoxService;

        bool _isFilenameDialogOpen;


        public FilesPage()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableFilesPageViewModel>();
            _mainViewModel = Service.Get<PortableMainViewModel>();
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _fileService = Service.Get<IFileService>();
            _messageBoxService = Service.Get<IMessageBoxService>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Loaded -= OnLoaded;
            Loaded += OnLoaded;
        }


        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isFirstLoopComplete && !App.IsPhone())
            {
                _isFirstLoopComplete = true;
                var timer = new DispatcherTimer();
                timer.Tick += WorkAroundLayoutBugWithMultipleScreens;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                timer.Start();
            }
            else
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                FilesListView.SelectionChanged -= OnListViewSelectionChanged;
                FilesListView.SelectionChanged += OnListViewSelectionChanged;

                SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                PageReference = this;
                await _viewModel.OnNavigatedToCompleteAndLoaded();
            }
        }

        void WorkAroundLayoutBugWithMultipleScreens(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();
            Frame.Navigate(typeof(FilesPage), new EntranceNavigationTransitionInfo());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Loaded -= OnLoaded;
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            FilesListView.SelectionChanged -= OnListViewSelectionChanged;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
        }

        async void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            if (_isFilenameDialogOpen)
            {
                var filenameDialogViewModel = Service.Get<PortableFilenameDialogViewModel>();
                if (filenameDialogViewModel.CancelCommand.CanExecute())
                    await filenameDialogViewModel.CancelCommand.Execute();
                return;
            }

            if (_viewModel.ClearPasswordAndExitCommand.CanExecute())
                await _viewModel.ClearPasswordAndExitCommand.Execute();
        }

        void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.IsEditMode))
            {
                if (_viewModel.IsEditMode)
                {
                    FilesListView.SelectionMode = ListViewSelectionMode.Multiple;
                    FilesListView.IsItemClickEnabled = false;
                }
                else
                {
                    FilesListView.SelectionMode = ListViewSelectionMode.None;
                    FilesListView.IsItemClickEnabled = true;
                }
            }
        }

        async void OnListViewItemClick(object sender, ItemClickEventArgs e)
        {
            var fileReference = e.ClickedItem as FileReference;
            await _viewModel.NavigateToOpenCommand.Execute(fileReference);
        }

        void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilesListView.SelectedItems.Count == 1)
            {
                _viewModel.SingleSelectedFileReference = FilesListView.SelectedItem as FileReference;
            }
            else if (FilesListView.SelectedItems.Count > 1)
            {
                _viewModel.MultiSelectedFileReferences = new List<FileReference>();
                foreach (var item in FilesListView.SelectedItems)
                {
                    _viewModel.MultiSelectedFileReferences.Add((FileReference)item);
                }
            }

            _viewModel.SelectedItemsCount = FilesListView.SelectedItems.Count;
            _viewModel.RenameCommand.RaiseCanExecuteChanged();
            _viewModel.DeleteCommand.RaiseCanExecuteChanged();
        }


        void DisplayDialogCommon()
        {
            FilesPageTopAppBar.Visibility = Visibility.Collapsed;

            FilesListView.IsEnabled = false;

            FilesPageBottomAppBar.Visibility = Visibility.Collapsed;
        }

        void HideDialogCommon()
        {
            FilesPageTopAppBar.Visibility = Visibility.Visible;
            FilesListView.IsEnabled = true;
            FilesListView.Focus(FocusState.Programmatic);

            FilesPageBottomAppBar.Visibility = Visibility.Visible;
        }

        public void DisplayFilenameDialog()
        {
            DisplayDialogCommon();
            FilenameUserControl.Visibility = Visibility.Visible;
            _isFilenameDialogOpen = true;
        }

        public void HideFilenameDialog()
        {
            HideDialogCommon();
            FilenameUserControl.Visibility = Visibility.Collapsed;
            _isFilenameDialogOpen = false;
        }

        async void OnButtonOpenFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var pickFileResult =
                    await
                        _fileService.PickFileAsync(null, DialogFilter.VisualCryptAndText, FileDialogMode.Open,
                            _resourceWrapper.miFileOpen.NoDots());
                if (pickFileResult.Item1)
                {
                    var storageFile = await StorageFile.GetFileFromPathAsync(pickFileResult.Item2);
                    var basicProperties = await storageFile.GetBasicPropertiesAsync();
                    var modifiedDate = basicProperties.DateModified.ToLocalTime().ToString();
                    var fileReference = new FileReference
                    {
                        ShortFilename = storageFile.Name,
                        PathAndFileName = storageFile.Path,
                        ModifiedDate = modifiedDate
                    };
                    await _viewModel.NavigateToOpenCommand.Execute(fileReference);
                }
            }
            catch (Exception ex)
            {
                await _messageBoxService.ShowError(ex);
            }

        }
    }
}
