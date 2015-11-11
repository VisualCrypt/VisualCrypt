using System;
using System.Globalization;
using System.Threading.Tasks;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;
using VisualCrypt.Windows.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VisualCrypt.Windows
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            Bootstrapper.Run();
        }

        protected async override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            Frame rootFrame = await GetOrCreateRootFrame();

            // TODO: Setup ShareTarget state

            Window.Current.Content = rootFrame;

            rootFrame.Navigate(typeof(MainPagePhone), args.ShareOperation);

            Bootstrapper.StopMeasureStartupTime();

            Window.Current.Activate();
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = await GetOrCreateRootFrame();

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                ;
            
            Window.Current.Content = rootFrame;
            
            if (rootFrame.Content == null)
                rootFrame.Navigate(typeof(FilesPage), e.Arguments);

            Bootstrapper.StopMeasureStartupTime();

            Window.Current.Activate();
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        async Task<Frame> GetOrCreateRootFrame()
        {
            #if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                DebugSettings.EnableFrameRateCounter = false;
            #endif

            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null)
                return rootFrame;

            rootFrame = new Frame();

            SetLanguage(rootFrame);

            await ConfigureUI();

            rootFrame.NavigationFailed += OnNavigationFailed;
            return rootFrame;
        }

        static void SetLanguage(Frame rootFrame)
        {
            // Set the default language
            var resourceWrapper = Service.Get<ResourceWrapper>();

            string languageResources = "en";
            string language = ApplicationLanguages.Languages[0];

            foreach (var bcp47LanguageTag in ApplicationLanguages.Languages)
            {
                var twoLetterISO = new CultureInfo(bcp47LanguageTag).TwoLetterISOLanguageName.ToLowerInvariant();

                if (resourceWrapper.Info.AvailableCultures.Contains(twoLetterISO))
                {
                    languageResources = twoLetterISO;
                    language = bcp47LanguageTag;
                    break;
                }
            }
            rootFrame.Language = language;
            resourceWrapper.Info.SwitchCulture(languageResources);
        }

        async Task ConfigureUI()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;

            var applicationView = ApplicationView.GetForCurrentView();
            var accentColor = Color.FromArgb(0xCC, 0xAF, 0x00, 0x07); // #CCAF0007 VisualCrypt Brand
            var accentColor2 = Color.FromArgb(0x99, 0xAF, 0x00, 0x07);
            var white = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            if (IsPhone())
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                if (Window.Current.Bounds.Width > Window.Current.Bounds.Height)
                    await statusBar.HideAsync();
                else
                    await statusBar.ShowAsync();
                //applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                statusBar.BackgroundColor = accentColor;
                statusBar.ForegroundColor = white;
                statusBar.BackgroundOpacity = 1;
                statusBar.ProgressIndicator.Text = "VisualCrypt";
                statusBar.ProgressIndicator.ProgressValue = 0;
                await statusBar.ProgressIndicator.ShowAsync();
                Window.Current.SizeChanged += async (sender, args) =>
                {


                    if (args.Size.Width > args.Size.Height)
                    {
                        await statusBar.HideAsync();
                    }
                    else
                    {
                        await statusBar.ShowAsync();
                    }

                };
            }

            var titleBar = applicationView.TitleBar;

            titleBar.BackgroundColor = accentColor;
            titleBar.ForegroundColor = white;
            titleBar.InactiveBackgroundColor = accentColor;
            titleBar.InactiveForegroundColor = white;
            titleBar.ButtonBackgroundColor = accentColor;
            titleBar.ButtonHoverBackgroundColor = accentColor2;
            titleBar.ButtonPressedBackgroundColor = accentColor2;
            titleBar.ButtonInactiveBackgroundColor = accentColor;
            titleBar.ButtonForegroundColor = white;
            titleBar.ButtonHoverForegroundColor = white;
            titleBar.ButtonPressedForegroundColor = white;
            titleBar.ButtonInactiveForegroundColor = white;

        }

        public static bool IsPhone()
        {
            return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
        }

        public static bool IsPhoneLayout()
        {
            return true;
        }


    }
}
