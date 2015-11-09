using System;
using System.Globalization;
using VisualCrypt.Windows.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;
using System.Resources;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.Foundation.Metadata;

namespace VisualCrypt.Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Bootstrapper.Run();

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            Service.Get<ResourceWrapper>().Info.SwitchCulture("de");
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                Service.Get<ResourceWrapper>().Info.SwitchCulture("de");
               
            }

           
            // Ensure the current window is active
            Window.Current.Activate();
           
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
  
            var applicationView = ApplicationView.GetForCurrentView();
            var accentColor = Color.FromArgb(0xCC, 0xAF, 0x00, 0x07); // #CCAF0007 VisualCrypt Brand
            var accentColor2 = Color.FromArgb(0x99, 0xAF, 0x00, 0x07);
            var white = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            if (IsPhone())
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                await statusBar.ShowAsync();
                //applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                statusBar.BackgroundColor = accentColor;
                statusBar.ForegroundColor = white;
                statusBar.BackgroundOpacity = 1;
                statusBar.ProgressIndicator.Text = "VisualCrypt";
                statusBar.ProgressIndicator.ProgressValue = 0;
                await statusBar.ProgressIndicator.ShowAsync();
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

            Service.Get<ILog>().Debug(string.Format(CultureInfo.InvariantCulture, "Loading completed after {0}ms.",
                  Bootstrapper.StopWatch.ElapsedMilliseconds));
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(FilesPage), e.Arguments);
            }
            Bootstrapper.StopWatch.Stop();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
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
