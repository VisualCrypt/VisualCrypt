using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.UWP.Styles;

namespace VisualCrypt.UWP.Pages
{
    sealed partial class MainPagePhone
    {
        public static MainPagePhone PageReference { get; private set; }

        // Set in constructor
        readonly PortableMainViewModel _viewModel;

        // Set in OnNavigatedTo (navigation parameters)
        FilesPageCommandArgs _filesPageCommandArgs;

        // Set in OnLoaded or in methods
        Dictionary<AppBarButton, double> _buttonsAndSizes;
        DataTransferManager _dataTransferManager;
        bool _isPasswordDialogOpen;
        bool _isFilenameDialogOpen;

        // Life Cycle: 0
        public MainPagePhone()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
        }

        // Life Cycle: 1
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Copy only the navigation parameters
            _filesPageCommandArgs = e.Parameter as FilesPageCommandArgs;

            // For everything else, wait till all elements have been contructed.
            Loaded -= OnLoaded;
            Loaded += OnLoaded;
        }

        // Life Cycle: 2
        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Unlike in OnNavigatedTo, the visual tree has now been constructed.

            // Subscribe to more events
            Window.Current.SizeChanged -= AdjustAppBarButtonSpacing;
            Window.Current.SizeChanged += AdjustAppBarButtonSpacing;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            // Update the globally accessible reference to this instance.
            PageReference = this;
            if (_filesPageCommandArgs.FilesPageCommand == FilesPageCommand.ShareTarget)
            {
                AppBarButtonBack.Visibility = Visibility.Collapsed;
                AppBarButtonShare.Visibility = Visibility.Collapsed;
            }
            // Finally, pass control to the ViewModel.
            await _viewModel.OnNavigatedToCompletedAndLoaded(_filesPageCommandArgs);
        }

        // Life Cycle 3:
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Loaded -= OnLoaded;
            Window.Current.SizeChanged -= AdjustAppBarButtonSpacing;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
        }

        // Do not forget to unregister the BackRequested event in OnNavigatedFrom!
        async void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;

            if (_isPasswordDialogOpen)
            {
                var passwordUserControlViewModel = Service.Get<PortablePasswordDialogViewModel>();
                if (passwordUserControlViewModel.CancelCommand.CanExecute())
                    await passwordUserControlViewModel.CancelCommand.Execute();
                return;
            }
            if (_isFilenameDialogOpen)
            {
                var filenameDialogViewModel = Service.Get<PortableFilenameDialogViewModel>();
                if (filenameDialogViewModel.CancelCommand.CanExecute())
                    await filenameDialogViewModel.CancelCommand.Execute();
                return;
            }

            if (_viewModel.GoBackToFilesCommand.CanExecute())
                await _viewModel.GoBackToFilesCommand.Execute();
        }



        void OnShareButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataTransferManager = DataTransferManager.GetForCurrentView();
                if (_dataTransferManager == null)
                    return;

                _dataTransferManager.DataRequested -= SendContentToShare;
                _dataTransferManager.DataRequested += SendContentToShare;
                DataTransferManager.ShowShareUI();
            }
            catch (Exception)
            {
                if (_dataTransferManager != null)
                    _dataTransferManager.DataRequested -= SendContentToShare;
            }
        }
        void SendContentToShare(DataTransferManager sender, DataRequestedEventArgs e)
        {
            try
            {
                var dataToSend = EditorUserControl.TextBox1.Text;
                e.Request.Data.Properties.Title = "VisualCrypt";
                e.Request.Data.SetText(dataToSend);
                e.Request.Data.RequestedOperation = DataPackageOperation.Copy;
               
            }
            catch (Exception)
            {
            }
            finally
            {
                if (_dataTransferManager != null)
                { 
                    _dataTransferManager.DataRequested -= SendContentToShare;
                }
            }
           
        }

        void OnPrintButtonClick(object sender, RoutedEventArgs e)
        {
            if (EditorUserControl.EditorViewModel.CanExecutePrint())
                EditorUserControl.EditorViewModel.ExecutePrint();
        }



        void AdjustAppBarButtonSpacing(object sender, WindowSizeChangedEventArgs e)
        {
            if (_buttonsAndSizes == null)
            {
                _buttonsAndSizes = new Dictionary<AppBarButton, double>();
                foreach (var commandBarElement in MainPageTopAppBar.PrimaryCommands)
                {
                    var button = commandBarElement as AppBarButton;
                    if (button != null) _buttonsAndSizes.Add(button, button.Width);
                }
            }

            if (App.IsLandscape(e))
            {
                foreach (var commandBarElement in MainPageTopAppBar.PrimaryCommands)
                {
                    var button = commandBarElement as AppBarButton;
                    if (button != null)
                        button.Width = 68;
                }
            }
            else
            {
                foreach (var pair in _buttonsAndSizes)
                {
                    pair.Key.Width = pair.Value;
                }
            }
        }

        void DisplayDialogCommon()
        {
            MainPageTopAppBar.Visibility = Visibility.Collapsed;
            ExtendedAppBar.Visibility = Visibility.Collapsed;

            EditorUserControl.TextBox1.IsEnabled = false;

            BottomBar.Opacity = 0.8;
        }

        void HideDialogCommon()
        {
            MainPageTopAppBar.Visibility = Visibility.Visible;
            ExtendedAppBar.Visibility = Visibility.Visible;
            EditorUserControl.TextBox1.IsEnabled = true;
            EditorUserControl.TextBox1.Focus(FocusState.Programmatic);
            BottomBar.Opacity = 1;
        }

        public void DisplayPasswordDialog()
        {
            DisplayDialogCommon();
            PasswordUserControl.Visibility = Visibility.Visible;
            _isPasswordDialogOpen = true;
        }

        public void HidePasswordDialog()
        {
            HideDialogCommon();
            PasswordUserControl.Visibility = Visibility.Collapsed;
            _isPasswordDialogOpen = false;
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
    }
}
