using System;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;

namespace VisualCrypt.Cryptography.Portable
{
    public interface IPasswordDialogDispatcher
    {

        /// <returns>true if the dialog was closed with 'Set'</returns>
        Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSet);
    }
}
