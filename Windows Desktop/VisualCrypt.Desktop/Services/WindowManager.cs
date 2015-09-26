using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Desktop.Controls;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop.Services
{
    public class WindowManager : IWindowManager
    {
        public async Task<object> ShowWindowAsyncAndWaitForClose<T>() where T : class, new()// AppWindow
        {
            var tcs = new TaskCompletionSource<object>();
            try
            {
                var appWindow = new T() as AppWindow;

                appWindow.ShowInTaskbar = true;
                EnsureCustomWindowConfiguration(appWindow);

                appWindow.Closed += (o, args) => tcs.SetResult(null);
                appWindow.Show();
                return await tcs.Task;
            }

            catch (Exception e)
            {
                tcs.SetException(e);
            }
            return await tcs.Task;
        }

        public async Task<T> GetDialogFromShowDialogAsyncWhenClosed<T>() where T : class, new()
        {
            var tcs = new TaskCompletionSource<T>();

            var appDialog = new T() as AppDialog;
            appDialog.ShowInTaskbar = false;
            EnsureCustomWindowConfiguration(appDialog);

            appDialog.ShowDialog();
            tcs.SetResult(appDialog as T);
            return await tcs.Task;
        }

        public async Task<Tuple<bool?, Encoding>> GetDialogFromShowDialogAsyncWhenClosed_ImportEncodingDialog()
        {
            var importEncodingDialog = await GetDialogFromShowDialogAsyncWhenClosed<ImportEncodingDialog>();

            var selectedEncoding = importEncodingDialog.SelectedEncodingInfo.GetEncoding();
            return new Tuple<bool?, Encoding>(importEncodingDialog.DialogResult, selectedEncoding);
        }

        public Task GetBoolFromShowDialogAsyncWhenClosed_SettingsDialog()
        {
            return GetBoolFromShowDialogAsyncWhenClosed<SettingsDialog>();
        }

        public Task ShowAboutDialogAsync()
        {
            return GetBoolFromShowDialogAsyncWhenClosed<AboutDialog>();
        }


        LogWindow _logWindow;

        public async Task ShowLogWindowAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            try
            {
                if (_logWindow == null)
                {
                    _logWindow = new LogWindow();
                    _logWindow.Closed += (s, e) => { _logWindow = null; };
                    _logWindow.ShowInTaskbar = true;
                    EnsureCustomWindowConfiguration(_logWindow);
                    _logWindow.ResizeMode = ResizeMode.CanResize;
                    _logWindow.Show();
                }
                else
                {
                    _logWindow.WindowState = WindowState.Normal;
                    _logWindow.Activate();
                }
                tcs.SetResult(null);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
            await tcs.Task;
            return;
        }

        public async Task<bool> GetBoolFromShowDialogAsyncWhenClosed<T>() where T : class, new()
        {
            var tcs = new TaskCompletionSource<bool>();

            var appDialog = new T() as Window;
            appDialog.ShowInTaskbar = false;
            EnsureCustomWindowConfiguration(appDialog);

            var result = appDialog.ShowDialog() == true;
            tcs.SetResult(result);
            return await tcs.Task;
        }

        static void EnsureCustomWindowConfiguration(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowStyle = WindowStyle.None;
            window.AllowsTransparency = true; // important, if not true, painting errors may occur!
            window.ResizeMode = ResizeMode.NoResize;
        }



    }
}