using System;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Views;

namespace VisualCrypt.Droid.Services
{
    class PasswordDialogDispatcher : IPasswordDialogDispatcher
    {
        public async Task<bool> LaunchAsync(SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSetViewModelCallback, bool isPasswordSetWhenDialogOpened)
        {
            var tcs = new TaskCompletionSource<bool>();
            MainActivity.MainActivityInstance.DisplayPasswordDialog(setPasswordDialogMode,
                setIsPasswordSetViewModelCallback, isPasswordSetWhenDialogOpened, tcs.SetResult);
            var result = await tcs.Task;
            MainActivity.MainActivityInstance.HidePasswordDialog();
            return result;
        }
    }
}