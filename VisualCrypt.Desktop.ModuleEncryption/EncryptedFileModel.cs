using System;
using System.Text;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Desktop.Shared.Files;

namespace VisualCrypt.Desktop.ModuleEncryption
{
    public class EncryptedFileModel : FileModelBase
    {
        readonly bool _isDirty1;
        readonly CipherV2 _cipherV2;
      

        public EncryptedFileModel(string visualCryptText, CipherV2 cipherV2, string filename)
        {
            Contents = visualCryptText;
            Filename = filename;
            _isDirty1 = false;

            SaveEncoding = new UTF8Encoding(false, true);
            _cipherV2 = cipherV2;
            IsEncrypted = true;
        }

        public override bool IsDirty
        {
            get { return _isDirty1; }
            set { /* do nothing! */ }
        }

        internal CipherV2 CipherV2 { get { return _cipherV2; } }
    }
}
