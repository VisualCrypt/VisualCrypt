using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using VisualCrypt.Desktop.Shared.Controls;

namespace VisualCrypt.Desktop.Shared.App
{
	public static class WindowManager
	{
		static readonly ILoggerFacade Logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();

		public static async Task<object> ShowWindowAsyncAndWaitForClose<T>() where T : AppWindow
		{
			var tcs = new TaskCompletionSource<object>();
			try
			{
				var appWindow = ServiceLocator.Current.GetInstance<T>();

				appWindow.ShowInTaskbar = true;
				EnsureCustomWindowConfiguration(appWindow);

				appWindow.Closed += (o, args) => tcs.SetResult(null);
				appWindow.Show();
				return await tcs.Task;
			}

			catch (Exception e)
			{
				Logger.Log(e.Message, Category.Exception, Priority.High);
				tcs.SetException(e);
			}
			return await tcs.Task;
		}

		public static async Task<T> GetDialogFromShowDialogAsyncWhenClosed<T>() where T : AppDialog
		{
			var tcs = new TaskCompletionSource<T>();

			var appDialog = ServiceLocator.Current.GetInstance<T>();
			appDialog.ShowInTaskbar = false;
			EnsureCustomWindowConfiguration(appDialog);

			appDialog.ShowDialog();
			tcs.SetResult(appDialog);
			return await tcs.Task;
		}


		public static async Task<bool> GetBoolFromShowDialogAsyncWhenClosed<T>() where T : Window
		{
			var tcs = new TaskCompletionSource<bool>();

			var appDialog = ServiceLocator.Current.GetInstance<T>();
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