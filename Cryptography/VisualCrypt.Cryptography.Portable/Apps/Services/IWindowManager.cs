using System;
using System.Text;
using System.Threading.Tasks;

namespace VisualCrypt.Cryptography.Portable.Apps.Services
{
	public interface IWindowManager
	{
		Task<object> ShowWindowAsyncAndWaitForClose<T>() where T : class; //AppWindow;
		Task<T> GetDialogFromShowDialogAsyncWhenClosed<T>() where T : class; //AppDialog;
		Task<bool> GetBoolFromShowDialogAsyncWhenClosed<T>() where T : class; //Window;
		Task<Tuple<bool?, Encoding>> GetDialogFromShowDialogAsyncWhenClosed_ImportEncodingDialog();

		Task GetBoolFromShowDialogAsyncWhenClosed_SettingsDialog();

		Task ShowAboutDialogAsync();

		Task ShowLogWindowAsync();
	}
}
