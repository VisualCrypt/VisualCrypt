using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using VisualCrypt.Applications.Portable.Apps.Models;
using VisualCrypt.Applications.Portable.Apps.Services;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;
using VisualCrypt.Desktop.Shared.PrismSupport;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop.Services
{
	[Export(typeof(IPasswordDialogDispatcher))]
	public class PasswordDialogDispatcher : IPasswordDialogDispatcher
	{
		readonly IWindowManager _windowManager;
		[ImportingConstructor]
		public PasswordDialogDispatcher(IWindowManager windowManager)
		{
			_windowManager = windowManager;
		}
		public async Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, 
			Action<bool> setIsPasswordSet, bool isPasswordSet)
		{
			ParamsProvider.SetParams(typeof(SetPasswordDialog), new Tuple<SetPasswordDialogMode,Action<bool>,bool>(setPasswordDialogMode,setIsPasswordSet,isPasswordSet));
			
			
			return await _windowManager.GetBoolFromShowDialogAsyncWhenClosed<SetPasswordDialog>();
		}
	}
}
