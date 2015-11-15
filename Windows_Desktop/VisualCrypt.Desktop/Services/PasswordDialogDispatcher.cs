using System;
using System.Threading.Tasks;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop.Services
{
	public class PasswordDialogDispatcher : IPasswordDialogDispatcher
	{
		readonly IWindowManager _windowManager;
	    readonly IParamsProvider _paramsProvider;

		public PasswordDialogDispatcher()
		{
			_windowManager = Service.Get<IWindowManager>();
            _paramsProvider = Service.Get<IParamsProvider>();
        }
		public async Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, 
			Action<bool> setIsPasswordSet, bool isPasswordSet)
		{
			_paramsProvider.SetParams(typeof(SetPasswordDialog), new Tuple<SetPasswordDialogMode,Action<bool>,bool>(setPasswordDialogMode,setIsPasswordSet,isPasswordSet));
			
			
			return await _windowManager.GetBoolFromShowDialogAsyncWhenClosed<SetPasswordDialog>();
		}
	}
}
