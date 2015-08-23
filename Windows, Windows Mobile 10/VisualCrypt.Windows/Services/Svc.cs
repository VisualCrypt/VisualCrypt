﻿using Windows.UI.Xaml.Controls;
using Prism.Events;
using VisualCrypt.Applications.Apps.Services;
using VisualCrypt.Applications.Apps.Settings;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Windows.Cryptography;
using VisualCrypt.Windows.Pages;

namespace VisualCrypt.Windows.Services
{
    static class Svc
    {
        public static readonly IEventAggregator EventAggregator = new EventAggregator();
        public static readonly ILog Log = new Log();

        public static readonly IMessageBoxService MessageBoxService = new MessageBoxService();
        public static readonly IEncryptionService EncryptionService = new EncryptionService();

        public static INavigationService NavigationService(Frame frame) => new NavigationService(frame);
        public static readonly IPasswordDialogDispatcher PasswordDialogDispatcher = new PasswordDialogDispatcher();

        public static readonly ISettingsManager SettingsManager = new SettingsManager();
        public static readonly IFileService FileService = new FileService();

        public static readonly IBrowserService BrowserService = new BrowserService();
        public static readonly IAssemblyInfoProvider AssemblyInfoProvider= new AssemblyInfoProvider(); 

        public static readonly ILifeTimeService LifeTimeService = new LifeTimeService();
        public static readonly IClipBoardService ClipBoardService = new ClipBoardService();
        public static readonly IWindowManager WindowManager = new WindowManager();

       
      
      
    }
}
