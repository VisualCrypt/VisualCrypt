﻿using System;
using System.Threading.Tasks;
using VisualCrypt.Applications.Portable.Apps.Models;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;

namespace VisualCrypt.Applications.Portable.Apps.Services
{
    public interface IPasswordDialogDispatcher
    {

        /// <returns>true if the dialog was closed with 'Set'</returns>
        Task<bool> LaunchAsync(IEncryptionService encryptionServcie, SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSet);
    }
}
