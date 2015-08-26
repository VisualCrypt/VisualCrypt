using System;
using System.Text;
using System.Threading.Tasks;

namespace VisualCrypt.Applications.Services.Interfaces
{
	public interface IWindowManager
	{
		Task<object> ShowWindowAsyncAndWaitForClose<T>() where T : class, new(); //AppWindow;
		Task<T> GetDialogFromShowDialogAsyncWhenClosed<T>() where T : class, new(); //AppDialog;
		Task<bool> GetBoolFromShowDialogAsyncWhenClosed<T>() where T : class, new(); //Window;
		Task<Tuple<bool?, Encoding>> GetDialogFromShowDialogAsyncWhenClosed_ImportEncodingDialog();

		Task GetBoolFromShowDialogAsyncWhenClosed_SettingsDialog();

		Task ShowAboutDialogAsync();

		Task ShowLogWindowAsync();
	}
}
