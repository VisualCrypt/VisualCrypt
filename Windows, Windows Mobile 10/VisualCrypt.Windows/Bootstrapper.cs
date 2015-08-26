using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Prism.Events;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Cryptography.UWP;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Windows.Pages;
using VisualCrypt.Windows.Services;

namespace VisualCrypt.Windows
{
    public static class Bootstrapper
    {
        public static readonly Stopwatch StopWatch = new Stopwatch();
      
        public static  void Run()
        {
            try
            {
                StopWatch.Start();
                ConfigureFactory();
                Service.Get<ISettingsManager>().LoadOrInitSettings();
            }
            catch (Exception e)
            {
                var md = new MessageDialog(e.Message);
                 md.ShowAsync();
            }
        }

        public static void ConfigureFactory()
        {

            Service.Register<ILog, ReplayLogger>(true);
            Service.Get<ILog>().Debug("Registering Services");

            Service.Register<IEventAggregator, EventAggregator>(true);
            Service.Register<IParamsProvider, ParamsProvider>(false);
            Service.Register<IMessageBoxService, MessageBoxService>(true);
            Service.Register<IEncryptionService, PortableEncryptionService>(true);
            Service.Register<INavigationService, NavigationService>(true);
            Service.Register<IPasswordDialogDispatcher, PasswordDialogDispatcher>(true);
            Service.Register<ISettingsManager, SettingsManager>(true);
            Service.Register<IFileService, FileService>(true);
            Service.Register<IBrowserService, BrowserService>(false);
            Service.Register<IAssemblyInfoProvider, AssemblyInfoProvider>(false);
            Service.Register<ILifeTimeService, LifeTimeService>(true);
            Service.Register<IClipBoardService, ClipBoardService>(true);
            Service.Register<IWindowManager, WindowManager>(true);
            Service.Register<IEncodingDetection, EncodingDetection>(true);

            Service.Register<PortableMainViewModel, PortableMainViewModel>(true);
            Service.Register<PortableEditorViewModel, PortableEditorViewModel>(true);
            Service.Register<FilesPageViewModel, FilesPageViewModel>(true);

            Service.Register<IFontManager, FontManager>(true);
            Service.Register<IPrinter, Printer>(true);

            Service.Register<IPlatform, Platform_UWP>(true);
            Service.Register<IVisualCrypt2Service, VisualCrypt2Service>(true);

            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBox1);
            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxFind);
            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxFindReplace);
            Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxGoTo);


            Service.Get<IVisualCrypt2Service>().Platform = Service.Get<IPlatform>();
            Service.Get<IEncodingDetection>().PlatformDefaultEncoding = null;
        }

      
    }
}