using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;
using VisualCrypt.UWP.Pages;
using VisualCrypt.UWP.Services;
using VisualCrypt.UWP.Styles;

namespace VisualCrypt.UWP
{
    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            Bootstrapper.Run();
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            try
            {
                if (args?.ShareOperation?.Data == null)
                    return;
                if (!args.ShareOperation.Data.Contains(StandardDataFormats.Text))
                {
                    args.ShareOperation.ReportError("VisualCrypt can only handle text data at this time.");
                    return;
                }

                var textReceived = await args.ShareOperation.Data.GetTextAsync();
                Bootstrapper.StopMeasureStartupTime();

                var rootFrame = await GetOrCreateRootFrame();

                await rootFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Window.Current.Content = rootFrame;
                    Service.Get<INavigationService>().NavigateToMainPage(new FilesPageCommandArgs
                    {
                        FilesPageCommand = FilesPageCommand.ShareTarget,
                        TextContents = textReceived
                    });
                    Window.Current.Activate();
                });

            }
            catch (Exception e)
            {
                args?.ShareOperation?.ReportError("VisualCrypt: " + e.Message);
            }
        }

        IStorageItem _file;
        BasicProperties _props;
        Frame _rootFrame;
        protected async override void OnFileActivated(FileActivatedEventArgs args)
        {
            try
            {
                _file = args.Files[0];
                _props = await _file.GetBasicPropertiesAsync();
                Bootstrapper.StopMeasureStartupTime();
                _rootFrame = await GetOrCreateRootFrame();
                Window.Current.Content = _rootFrame;
                Window.Current.Activate();

                var timer = new DispatcherTimer();
                timer.Tick += WorkAroundLayoutBugWithMultipleScreens;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                timer.Start();
            }
            catch (Exception)
            {

            }
        }

        async void WorkAroundLayoutBugWithMultipleScreens(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();
            await _rootFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var fileservice = (FileService)Service.Get<IFileService>();
                string fileToken = StorageApplicationPermissions.FutureAccessList.Add(_file, _file.Path);
                fileservice.AccessTokens[_file.Path] = fileToken; // add or replace
               
                Service.Get<INavigationService>().NavigateToMainPage(new FilesPageCommandArgs
                {
                    FilesPageCommand = FilesPageCommand.Open,
                    FileReference = new FileReference
                    {
                        FileSystemObject = _file,
                        ModifiedDate = _props.DateModified.ToString(),
                        PathAndFileName = _file.Path
                    }
                });

            });
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = await GetOrCreateRootFrame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(FilesPage), e.Arguments);
            Window.Current.Activate();
            Bootstrapper.StopMeasureStartupTime();
        }


        static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            var frame = Window.Current.Content as Frame ?? new Frame();
            frame.Navigate(typeof(FilesPage));
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        static async Task<Frame> GetOrCreateRootFrame()
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                Current.DebugSettings.EnableFrameRateCounter = false;
#endif

            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null)
                return rootFrame;

            rootFrame = new Frame();

            SetLanguage(rootFrame);

            await ConfigureUI();
            rootFrame.NavigationFailed -= OnNavigationFailed;
            rootFrame.NavigationFailed += OnNavigationFailed;
            return rootFrame;
        }

        static void SetLanguage(Frame rootFrame)
        {
            var resourceWrapper = Service.Get<ResourceWrapper>();

            var languageResources = "en";
            var language = ApplicationLanguages.Languages[0];

            foreach (var bcp47LanguageTag in ApplicationLanguages.Languages)
            {
                var twoLetterISO = new CultureInfo(bcp47LanguageTag).TwoLetterISOLanguageName.ToLowerInvariant();
                if (!resourceWrapper.Info.AvailableCultures.Contains(twoLetterISO))
                    continue;

                languageResources = twoLetterISO;
                language = bcp47LanguageTag;
                break;
            }
            rootFrame.Language = language;
            resourceWrapper.Info.SwitchCulture(languageResources);
        }

        public static bool IsLandscape(WindowSizeChangedEventArgs args)
        {
            return args.Size.Width > args.Size.Height;
        }

        static async Task ConfigureUI()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = MoreColors.AccentColorBrush.Color;
            titleBar.InactiveBackgroundColor = MoreColors.AccentColorBrush.Color;
            titleBar.ButtonBackgroundColor = MoreColors.AccentColorBrush.Color;
            titleBar.ButtonInactiveBackgroundColor = MoreColors.AccentColorBrush.Color;

            titleBar.InactiveForegroundColor = MoreColors.WhiteBrush.Color;
            titleBar.ForegroundColor = MoreColors.WhiteBrush.Color;
            titleBar.ButtonForegroundColor = MoreColors.WhiteBrush.Color;
            titleBar.ButtonHoverForegroundColor = MoreColors.WhiteBrush.Color;
            titleBar.ButtonPressedForegroundColor = MoreColors.WhiteBrush.Color;
            titleBar.ButtonInactiveForegroundColor = MoreColors.WhiteBrush.Color;

            titleBar.ButtonHoverBackgroundColor = MoreColors.AccentColorBrush2.Color;
            titleBar.ButtonPressedBackgroundColor = MoreColors.AccentColorBrush2.Color;


            if (IsPhone())
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (Window.Current.Bounds.Width > Window.Current.Bounds.Height)
                    await statusBar.HideAsync();
                else
                    await statusBar.ShowAsync();
                //applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                statusBar.BackgroundColor = MoreColors.AccentColorBrush.Color;
                statusBar.ForegroundColor = MoreColors.WhiteBrush.Color;
                statusBar.BackgroundOpacity = 1;
                statusBar.ProgressIndicator.Text = "VisualCrypt";
                statusBar.ProgressIndicator.ProgressValue = 0;
                await statusBar.ProgressIndicator.ShowAsync();
                Window.Current.SizeChanged += async (sender, args) =>
                {
                    if (IsLandscape(args))
                    {
                        await statusBar.HideAsync();
                    }
                    else
                    {
                        await statusBar.ShowAsync();
                    }
                };
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }

        public static bool IsPhone()
        {
            return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
        }



    }
}
