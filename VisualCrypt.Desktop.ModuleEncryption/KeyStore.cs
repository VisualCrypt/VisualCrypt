using System;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Desktop.ModuleEncryption
{
    static class KeyStore
    {
        static SHA256PW32 _sha256Pw32;

        public static SHA256PW32 GetSHA256PW32()
        {
            if(_sha256Pw32 == null)
                throw new InvalidOperationException("KeyStore: _sha256Pw32 is null.");
            return _sha256Pw32; 
        }

        public static void SetSHA256PW32(SHA256PW32 sha256Pw32)
        {
            _sha256Pw32 = sha256Pw32;
        }

        public static void Clear()
        {
            if(_sha256Pw32 != null)
                _sha256Pw32.Value.OverwriteWithZeros();
            _sha256Pw32 = null;
        }
    }
}
