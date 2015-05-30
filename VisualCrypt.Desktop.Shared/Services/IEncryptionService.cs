using System;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Desktop.Shared.Files;

namespace VisualCrypt.Desktop.Shared.Services
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Returns a clear text file or an encrypted file.
        /// </summary>
        Response<FileModelBase> OpenFile(string filename);

        Response SetPassword(byte[] utf16LEPassword);

        Response ClearPassword();
    }
}
