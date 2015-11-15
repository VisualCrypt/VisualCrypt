using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
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
        ShareOperation _shareOperation;
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
            _shareOperation = e.Parameter as ShareOperation;
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

            // behave like a share target...
            if (_shareOperation != null)
                await HandleAsShareTarget(_shareOperation);
            else  // ...or register the current page as a share source.
            {
                _dataTransferManager = DataTransferManager.GetForCurrentView();
                _dataTransferManager.DataRequested -= SendContentToShare;
                _dataTransferManager.DataRequested += SendContentToShare;
            }

            // Update the globally accessible reference to this instance.
            PageReference = this;

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
            if (_dataTransferManager != null)
                _dataTransferManager.DataRequested -= SendContentToShare;
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
            DataTransferManager.ShowShareUI();
        }

        void OnPrintButtonClick(object sender, RoutedEventArgs e)
        {
            if (EditorUserControl._viewModel.CanExecutePrint())
                EditorUserControl._viewModel.ExecutePrint();
        }


        async Task HandleAsShareTarget(ShareOperation shareOperation)
        {
            if (shareOperation == null)
                return;

            await Task.Factory.StartNew(async () =>
            {
                if (shareOperation.Data.Contains(StandardDataFormats.Text))
                {
                    try
                    {
                        var sharedText = await shareOperation.Data.GetTextAsync();
                        await
                            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () => { EditorUserControl.TextBox1.Text = sharedText; });
                    }
                    catch (Exception ex)
                    {
                        NotifyUserBackgroundThread("Failed GetTextAsync - " + ex.Message);
                    }
                }
            });
        }

        async void NotifyUserBackgroundThread(string message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Service.Get<IMessageBoxService>().ShowError(message);
            });
        }

        void SendContentToShare(DataTransferManager sender, DataRequestedEventArgs e)
        {
            e.Request.Data.Properties.Title = "VisualCrypt";

            e.Request.Data.Properties.ContentSourceApplicationLink = new Uri("ms-sdk-sharesourcecs:navigate?page=" + GetType().Name);
            e.Request.Data.SetText(EditorUserControl.TextBox1.Text);
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
            EditorUserControl.TextBox1.Opacity = 0.8;

            BottomBar.Opacity = 0.8;
            BackgroundGrid.Background = MoreColors.BackgroundGridDisabledColorBrush;
        }

        void HideDialogCommon()
        {
            MainPageTopAppBar.Visibility = Visibility.Visible;
            ExtendedAppBar.Visibility = Visibility.Visible;
            EditorUserControl.TextBox1.IsEnabled = true;
            EditorUserControl.TextBox1.Opacity = 1;
            EditorUserControl.TextBox1.Focus(FocusState.Programmatic);
            BottomBar.Opacity = 1;
            BackgroundGrid.Background = MoreColors.WhiteBrush;
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
