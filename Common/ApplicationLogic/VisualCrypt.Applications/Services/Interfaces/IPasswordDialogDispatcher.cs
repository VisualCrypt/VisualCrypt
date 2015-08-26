using System;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IPasswordDialogDispatcher
    {

        /// <returns>true if the dialog was closed with 'Set'</returns>
        Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSet);
    }
}
