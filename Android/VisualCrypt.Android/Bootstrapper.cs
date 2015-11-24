using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Prism.Events;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Cryptography.Net;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Droid.Models;
using VisualCrypt.Droid.Services;
using VisualCrypt.Language.Strings;
using IWindowManager = Android.Views.IWindowManager;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;

namespace VisualCrypt.Droid
{
    class Bootstrapper
    {
        public static void Register()
        {

            Applications.Services.Interfaces.Service.Register<ILog, ReplayLogger>(true);
            Applications.Services.Interfaces.Service.Get<ILog>().Debug("Registering Services");

            Applications.Services.Interfaces.Service.Register<AbstractSettingsManager, SettingsManager>(true);

            Applications.Services.Interfaces.Service.Register<ResourceWrapper, ResourceWrapper>(true);

            Applications.Services.Interfaces.Service.Register<IEventAggregator, EventAggregator>(true);
            Applications.Services.Interfaces.Service.Register<IParamsProvider, ParamsProvider>(false);
            Applications.Services.Interfaces.Service.Register<IMessageBoxService, MessageBoxService>(true);
            Applications.Services.Interfaces.Service.Register<IEncryptionService, PortableEncryptionService>(true);
            Applications.Services.Interfaces.Service.Register<INavigationService, NavigationService>(true);
            Applications.Services.Interfaces.Service.Register<IPasswordDialogDispatcher, PasswordDialogDispatcher>(true);


            Applications.Services.Interfaces.Service.Register<IFontSettings, FontSettings>(true);

            Applications.Services.Interfaces.Service.Register<IFileService, FileService>(true);
            Applications.Services.Interfaces.Service.Register<IAssemblyInfoProvider, AssemblyInfoProvider>(true);
            Applications.Services.Interfaces.Service.Get<IAssemblyInfoProvider>().Assembly = typeof(Bootstrapper).Assembly;
            Applications.Services.Interfaces.Service.Register<ILifeTimeService, LifeTimeService>(true);
            Applications.Services.Interfaces.Service.Register<IClipBoardService, ClipBoardService>(true);
            Applications.Services.Interfaces.Service.Register<IEncodingDetection, EncodingDetection>(true);

            Applications.Services.Interfaces.Service.Register<PortableMainViewModel, PortableMainViewModel>(true);
            Applications.Services.Interfaces.Service.Register<PortableEditorViewModel, PortableEditorViewModel>(true);
            Applications.Services.Interfaces.Service.Register<PortablePasswordDialogViewModel, PortablePasswordDialogViewModel>(true);
            Applications.Services.Interfaces.Service.Register<PortableFilenameDialogViewModel, PortableFilenameDialogViewModel>(true);
            Applications.Services.Interfaces.Service.Register<PortableFilesPageViewModel, PortableFilesPageViewModel>(true);

            Applications.Services.Interfaces.Service.Register<IFontManager, FontManager>(true);
            Applications.Services.Interfaces.Service.Register<IPrinter, Printer>(true);

            Applications.Services.Interfaces.Service.Register<IPlatform, Platform_Net4>(true);
            Applications.Services.Interfaces.Service.Register<IVisualCrypt2Service, VisualCrypt2Service>(true);

            Applications.Services.Interfaces.Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBox1);
            Applications.Services.Interfaces.Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxFind);
            Applications.Services.Interfaces.Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxFindReplace);
            Applications.Services.Interfaces.Service.Register<ITextBoxController, TextBoxController>(true, TextBoxName.TextBoxGoTo);



            Applications.Services.Interfaces.Service.Get<IVisualCrypt2Service>().Platform = Applications.Services.Interfaces.Service.Get<IPlatform>();
            Service.Get<IEncodingDetection>().PlatformDefaultEncoding = null;
        }
    }
}