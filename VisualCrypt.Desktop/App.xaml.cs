using System;
using System.Reflection;
using System.Windows;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop
{
    public partial class App
    {
        void Application_Startup(object sender, StartupEventArgs startupEventArgs)
        {
            try
            {
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                var mainWindow = new MainWindow();
                mainWindow.Show();
                var mainWindowViewModel = (MainWindowViewModel)mainWindow.DataContext;
                mainWindowViewModel.MessageBoxService = new MessageBoxService(mainWindow);
                mainWindowViewModel.OpenFileFromCommandLine(startupEventArgs.Args);
            }
            catch (Exception e)
            {
                new MessageBoxService().ShowError(MethodBase.GetCurrentMethod(), e);
               
                Environment.Exit(1);
            }
        }



        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            new MessageBoxService().ShowError(MethodBase.GetCurrentMethod(), e.Exception);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception ?? new Exception("Unknown error.");
            new MessageBoxService().ShowError(MethodBase.GetCurrentMethod(), exception);
        }
    }
}