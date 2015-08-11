using System;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.PrismSupport;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop.Services
{
	public class PasswordDialogDispatcher : IPasswordDialogDispatcher
	{
		public async Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, 
			Action<bool> setIsPasswordSet, bool isPasswordSet)
		{
			ParamsProvider.SetParams(typeof(SetPasswordDialog), setPasswordDialogMode);
			return await WindowManager.GetBoolFromShowDialogAsyncWhenClosed<SetPasswordDialog>();
		}
	}
}
