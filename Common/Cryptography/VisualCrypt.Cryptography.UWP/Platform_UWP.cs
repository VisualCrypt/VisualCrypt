using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using System;

namespace VisualCrypt.Cryptography.UWP
{
    public class Platform_UWP : IPlatform
    {
        public byte[] GenerateRandomBytes(int length)
        {
            IBuffer random = CryptographicBuffer.GenerateRandom((uint)length);
            return random.ToArray();
        }

        public byte[] ComputeSHA512(byte[] data)
        {
            Guard.NotNull(data);

            IBuffer input = CryptographicBuffer.CreateFromByteArray(data);

            var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA512");
            IBuffer hashed = hasher.HashData(input);
            return hashed.ToArray();
        }

        public byte[] ComputeSHA256(byte[] data)
        {
            Guard.NotNull(data);

            IBuffer input = CryptographicBuffer.CreateFromByteArray(data);

            var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA256");
            IBuffer hashed = hasher.HashData(input);
            return hashed.ToArray();
        }

        public byte[] ComputeAESRound(AESDir aesDir, byte[] currentIV, byte[] inputData, byte[] keyBytes)
        {
            Guard.NotNull(new object[] { currentIV, inputData, keyBytes });
            if (inputData.Length == 0)
                throw new NotSupportedException("Encryption of zero-lenght data is not supported on Windows Universal.");

            IBuffer iv = CryptographicBuffer.CreateFromByteArray(currentIV);

            IBuffer key = CryptographicBuffer.CreateFromByteArray(keyBytes);

            IBuffer data = CryptographicBuffer.CreateFromByteArray(inputData);

            var algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");  // Padding: None

            var symmKey = algorithm.CreateSymmetricKey(key);

            var resultBuffer = aesDir == AESDir.Encrypt
                ? CryptographicEngine.Encrypt(symmKey, data, iv)
                : CryptographicEngine.Decrypt(symmKey, data, iv);

            byte[] cipherBytes;
            CryptographicBuffer.CopyToByteArray(resultBuffer, out cipherBytes);
            return cipherBytes;
        }
    }
}
