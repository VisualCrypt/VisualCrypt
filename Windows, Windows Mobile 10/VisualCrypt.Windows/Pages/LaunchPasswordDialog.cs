using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using VisualCrypt.Applications.Portable.Apps.Models;
using VisualCrypt.Applications.Portable.Apps.Services;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using VisualCrypt.Windows.Controls;

namespace VisualCrypt.Windows.Pages
{
    class PasswordDialogDispatcher : IPasswordDialogDispatcher
    {
        Popup _passwordPopup;
        TaskCompletionSource<bool> _popupTaskCompletionSource;
        public async Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSet)
        {
            _passwordPopup = new Popup();
            _passwordPopup.Child = new PasswordDialog(encryptionServcie, setPasswordDialogMode,
                ClosePasswordPopop, setIsPasswordSet, isPasswordSet);
            _passwordPopup.IsOpen = true;

            _popupTaskCompletionSource = new TaskCompletionSource<bool>();
            return await _popupTaskCompletionSource.Task;
        }
        void ClosePasswordPopop(bool setClicked)
        {
            _popupTaskCompletionSource.SetResult(setClicked);
            _passwordPopup.IsOpen = false;
        }
    }

   
}
