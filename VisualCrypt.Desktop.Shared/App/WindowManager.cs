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

		//  for TaskCompletitionSource 
		public struct Void
		{
		}

		public static async Task<Void> ShowWindowAsyncAndWaitForClose<T>() where T : AppWindow
		{
			var tcs = new TaskCompletionSource<Void>();
			try
			{
				var appWindow = ServiceLocator.Current.GetInstance<T>();
				appWindow.Owner = Application.Current.MainWindow;
				appWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				appWindow.ShowInTaskbar = true;

				appWindow.Closed += (o, args) => tcs.SetResult(new Void());
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
			appDialog.Owner = Application.Current.MainWindow;
			appDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			appDialog.ShowInTaskbar = false;

			appDialog.Closed += (o, args) => tcs.SetResult(appDialog);
			appDialog.ShowDialog();

			return await tcs.Task.ConfigureAwait(false);
		}


		public static async Task<bool> GetBoolFromShowDialogAsyncWhenClosed<T>() where T : AppDialog
		{
			var tcs = new TaskCompletionSource<bool>();

			var appDialog = ServiceLocator.Current.GetInstance<T>();
			appDialog.Owner = Application.Current.MainWindow;
			appDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			appDialog.ShowInTaskbar = false;

			appDialog.Closed += (o, args) => tcs.SetResult(appDialog.DialogResult == true);
			appDialog.ShowDialog();

			return await tcs.Task.ConfigureAwait(false);
		}
	}
}