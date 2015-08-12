using System;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable.Apps.Models;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;

namespace VisualCrypt.Cryptography.Portable.Apps.Services
{
    public interface IPasswordDialogDispatcher
    {

        /// <returns>true if the dialog was closed with 'Set'</returns>
        Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSet);
    }
}
