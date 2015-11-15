using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Prism.Events;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Cryptography.UWP;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Language.Strings;
using VisualCrypt.UWP.Models;
using VisualCrypt.UWP.Pages;
using VisualCrypt.UWP.Services;

namespace VisualCrypt.UWP
{
    public static class Bootstrapper
    {
        public static readonly Stopwatch StopWatch = new Stopwatch();
      
        public static async  void Run()
        {
            try
            {
                StopWatch.Start();
                Register();
                Service.Get<AbstractSettingsManager>();
             
            }
            catch (Exception e)
            {
                Service.Get<ILog>().Exception(e);
            }
        }

        internal static void StopMeasureStartupTime()
        {
            Service.Get<ILog>().Debug(string.Format(CultureInfo.InvariantCulture, "Loading completed after {0}ms.",
                   StopWatch.ElapsedMilliseconds));
            StopWatch.Stop();
        }

        public static void Register()
        {

            Service.Register<ILog, ReplayLogger>(true);
            Service.Get<ILog>().Debug("Registering Services");

            Service.Register<AbstractSettingsManager, SettingsManager>(true);

            Service.Register<ResourceWrapper, ResourceWrapper>(true);

            Service.Register<IEventAggregator, EventAggregator>(true);
            Service.Register<IParamsProvider, ParamsProvider>(false);
            Service.Register<IMessageBoxService, MessageBoxService>(true);
            Service.Register<IEncryptionService, PortableEncryptionService>(true);
            Service.Register<INavigationService, NavigationService>(true);
            Service.Register<IPasswordDialogDispatcher, PasswordDialogDispatcher>(true);

          
            Service.Register<IFontSettings, FontSettings>(true);

            Service.Register<IFileService, FileService>(true);
            Service.Register<IAssemblyInfoProvider, AssemblyInfoProvider>(true);
            Service.Get<IAssemblyInfoProvider>().Assembly = typeof(Bootstrapper).GetTypeInfo().Assembly;
            Service.Register<ILifeTimeService, LifeTimeService>(true);
            Service.Register<IClipBoardService, ClipBoardService>(true);
            Service.Register<IWindowManager, WindowManager>(true);
            Service.Register<IEncodingDetection, EncodingDetection>(true);

            Service.Register<PortableMainViewModel, PortableMainViewModel>(true);
            Service.Register<PortableEditorViewModel, PortableEditorViewModel>(true);
            Service.Register<PortablePasswordDialogViewModel, PortablePasswordDialogViewModel>(true);
            Service.Register<PortableFilenameDialogViewModel, PortableFilenameDialogViewModel>(true);
            Service.Register<PortableFilesPageViewModel, PortableFilesPageViewModel>(true);

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