using System;
using System.Threading.Tasks;
using VisualCrypt.Applications.Apps.Models;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.Applications.Apps.Services
{
    public interface IPasswordDialogDispatcher
    {

        /// <returns>true if the dialog was closed with 'Set'</returns>
        Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSet);
    }
}
