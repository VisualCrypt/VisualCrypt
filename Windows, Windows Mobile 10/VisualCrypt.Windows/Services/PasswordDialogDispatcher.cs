using System;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Windows.Controls;
using VisualCrypt.Windows.Pages;
using Windows.UI.Xaml;

namespace VisualCrypt.Windows.Services
{
    public class PasswordDialogDispatcher : IPasswordDialogDispatcher
    {
       
        public async Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, 
            Action<bool> setIsPasswordSet, bool isPasswordSet)
        {
            var mainPage = MainPagePhone.GetMainPageReference();
            var tcs = new TaskCompletionSource<bool>();
            mainPage.PasswordUserControl.InitViewModel(encryptionServcie, setPasswordDialogMode,
                setIsPasswordSet, tcs.SetResult, isPasswordSet);
                
            mainPage.DisplayPasswordDialog();
            var result = await tcs.Task;
            mainPage.HidePasswordDialog();
            return result;
           
        }       
    }
}
