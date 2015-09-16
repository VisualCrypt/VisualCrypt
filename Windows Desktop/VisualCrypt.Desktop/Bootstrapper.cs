using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Prism.Events;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Cryptography.Net;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Desktop.Services;
using VisualCrypt.Desktop.Views;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Desktop.Settings;
using VisualCrypt.Language.Strings;
using System.Threading;

namespace VisualCrypt.Desktop
{
    public class Bootstrapper
    {

        static readonly Stopwatch StopWatch = new Stopwatch();

        [STAThread]
        public static void Main()
        {
            try
            {
                StopWatch.Start();

                ConfigureFactory();
                var systemCulture = Thread.CurrentThread.CurrentUICulture;
                var resourceWrapper = Service.Get<ResourceWrapper>();
                if (resourceWrapper.Info.AvailableCultures.Contains(systemCulture.TwoLetterISOLanguageName.ToLowerInvariant()))
                    resourceWrapper.Info.SwitchCulture(systemCulture.TwoLetterISOLanguageName.ToLowerInvariant());
                else resourceWrapper.Info.SwitchCulture("en");

                var app = new App();
                app.Startup += App_Startup;
                app.DispatcherUnhandledException += App_DispatcherUnhandledException;
                app.Run();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, nameof(Main));
            }
        }

        public static void ConfigureFactory()
        {

            Service.Register<ILog, ReplayLogger>(true);
            Service.Get<ILog>().Debug("Registering Services.");

            Service.Register<AbstractSettingsManager, SettingsManager>(true);
            Service.Register<IFontSettings, FontSettings>(false);

            Service.Register<IEventAggregator, EventAggregator>(true);
            Service.Register<IParamsProvider, ParamsProvider>(false);
            Service.Register<IMessageBoxService, MessageBoxService>(true);
            Service.Register<IEncryptionService, PortableEncryptionService>(true);
            Service.Register<INavigationService, NavigationService>(true);
            Service.Register<IPasswordDialogDispatcher, PasswordDialogDispatcher>(true);
          
            Service.Register<IFileService, FileService>(true);
            Service.Register<IBrowserService, BrowserService>(false);

            Service.Register<IAssemblyInfoProvider, AssemblyInfoProvider>(true);
            Service.Get<IAssemblyInfoProvider>().Assembly = typeof(Bootstrapper).Assembly;


            Service.Register<ILifeTimeService, LifeTimeService>(true);
            Service.Register<IClipBoardService, ClipBoardService>(true);
            Service.Register<IWindowManager, WindowManager>(true);
            Service.Register<IEncodingDetection, EncodingDetection>(true);

            Service.Register<PortableMainViewModel, PortableMainViewModel>(true);
            Service.Register<PortableEditorViewModel, PortableEditorViewModel>(true);

            Service.Register<IFontManager, FontManager>(true);
            Service.Register<IPrinter, Printer>(true);

            Service.Register<IPlatform, Platform_Net4>(true);
            Service.Register<IVisualCrypt2Service, VisualCrypt2Service>(true);

            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBox1);
            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxFind);
            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxFindReplace);
            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxGoTo);

            Service.Register<ResourceWrapper, ResourceWrapper>(true);

            Service.Get<IVisualCrypt2Service>().Platform = Service.Get<IPlatform>();
            Service.Get<IEncodingDetection>().PlatformDefaultEncoding = Encoding.Default;
        }

        static void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();

            Service.Get<ILog>().Debug(string.Format(CultureInfo.InvariantCulture, "Loading completed after {0}ms.",
                    StopWatch.ElapsedMilliseconds));
            StopWatch.Start();

            var args = Environment.GetCommandLineArgs();
            ((PortableMainViewModel)Application.Current.MainWindow.DataContext).OpenFromCommandLineOrNew(args);
        }

        static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, nameof(App_DispatcherUnhandledException));
            e.Handled = true;
            Environment.Exit(1);
        }
    }
}