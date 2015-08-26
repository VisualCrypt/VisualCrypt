using System;
using System.IO;
using System.Security.Cryptography;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.Cryptography.Net
{
	public class Platform_Net4 : IPlatform
	{
		public byte[] GenerateRandomBytes(int length)
		{
			if (length < 1)
				throw new ArgumentOutOfRangeException("length");

			var randomBytes = new byte[length];

			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(randomBytes);
				return randomBytes;
			}
		}

		public byte[] ComputeAESRound(AESDir aesDir, byte[] iv, byte[] input, byte[] keyBytes)
		{
			Guard.NotNull(new object[] { iv, input, keyBytes });

			switch (aesDir)
			{
				case AESDir.Encrypt:
					using (var aes = CreateAesManaged(iv, keyBytes))
					using (var stream = new MemoryStream())
					{
						using (var encryptor = aes.CreateEncryptor())
						using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
						{
							encrypt.Write(input, 0, input.Length);
							encrypt.FlushFinalBlock();
						}
						return stream.ToArray();
					}
				case AESDir.Decrpyt:
					using (var aes = CreateAesManaged(iv, keyBytes))
					using (var stream = new MemoryStream())
					{
						using (var decryptor = aes.CreateDecryptor())
						using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
						{
							decrypt.Write(input, 0, input.Length);
							decrypt.FlushFinalBlock();
						}
						return stream.ToArray();
					}
			}
			throw new InvalidOperationException(string.Format("{0} is not supported", aesDir));
		}

		public byte[] ComputeSHA512(byte[] data)
		{
			Guard.NotNull(data);

			using (var sha = new SHA512Managed())
			{
				return sha.ComputeHash(data);
			}
		}

		public byte[] ComputeSHA256(byte[] data)
		{
			Guard.NotNull(data);

			using (var sha = new SHA256Managed())
			{
				return sha.ComputeHash(data);
			}
		}

        static AesManaged CreateAesManaged(byte[] iv, byte[] keyBytes)
        {
            Guard.NotNull(new object[] { iv, keyBytes });

            return new AesManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Padding = PaddingMode.None,
                IV = iv,
                Key = keyBytes,
                Mode = CipherMode.CBC
            };
        }
    }
}