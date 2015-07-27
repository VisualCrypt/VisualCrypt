﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Cryptography.Net.APIV2.Implementations
{
	public class CoreAPIV2_Net4 : ICoreAPIV2
	{
		public Compressed Compress(ClearText clearText)
		{
			if (clearText == null)
				throw new ArgumentNullException("clearText");

			var deflate = new Deflate();
			byte[] compressed = deflate.Compress(clearText.Value, Encoding.UTF8);

			return new Compressed(compressed);
		}

		public PaddedData ApplyRandomPadding(Compressed compressed)
		{
			if (compressed == null)
				throw new ArgumentNullException("compressed");

			int requiredPadding;
			if (compressed.Value.Length % 16 == 0)
				requiredPadding = 0;
			else
				requiredPadding = 16 - compressed.Value.Length % 16;

			var paddingBytes = new byte[requiredPadding];

			using (var rng = new RNGCryptoServiceProvider())
				rng.GetBytes(paddingBytes);

			var paddedDataBytes = new byte[compressed.Value.Length + requiredPadding];
			Buffer.BlockCopy(compressed.Value, 0, paddedDataBytes, 0, compressed.Value.Length);
			Buffer.BlockCopy(paddingBytes, 0, paddedDataBytes, compressed.Value.Length, paddingBytes.Length);

			return new PaddedData(paddedDataBytes, requiredPadding);
		}

		public byte[] GenerateRandomBytes(int length)
		{
			var randomBytes = new byte[length];

			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(randomBytes);
				return randomBytes;
			}
		}

		public void AESEncryptRandomKeyWithPasswordDerivedKey(PasswordDerivedKey32 passwordDerivedKey, RandomKey32 randomKey, CipherV2 cipherV2)
		{
			if (passwordDerivedKey == null)
				throw new ArgumentNullException("passwordDerivedKey");

			if (randomKey == null)
				throw new ArgumentNullException("randomKey");

			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			cipherV2.RandomKeyCipher = ComputeAES(AESDir.Encrypt, cipherV2.IV16, randomKey.Value, passwordDerivedKey.Value, cipherV2.BWF.Value);
			
		}


		public void AESEncryptMessageWithRandomKey(PaddedData paddedData, RandomKey32 randomKey, CipherV2 cipherV2)
		{
			if (paddedData == null)
				throw new ArgumentNullException("paddedData");

			if (randomKey == null)
				throw new ArgumentNullException("randomKey");

			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (cipherV2.IV16 == null)
				throw new ArgumentException("cipherV2.IV16 must not be null at this point.");

			cipherV2.Padding = paddedData.Padding;

			cipherV2.MessageCipher = ComputeAES(AESDir.Encrypt, cipherV2.IV16, paddedData.DataBytes, randomKey.Value,cipherV2.BWF.Value);

		}


		public void AESEncryptMACWithRandomKey(CipherV2 cipherV2, MAC16 mac, RandomKey32 randomKey)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (mac == null)
				throw new ArgumentNullException("mac");

			if (randomKey == null)
				throw new ArgumentNullException("randomKey");

			if (cipherV2.IV16 == null)
				throw new ArgumentException("cipherV2.IV16 must not be null at this point.");

			cipherV2.MACCipher = ComputeAES(AESDir.Encrypt, cipherV2.IV16, mac.Value, randomKey.Value, cipherV2.BWF.Value);
		}

		static byte[] ComputeAES(AESDir aesDir, IV16 iv, byte[] dataBytes, byte[] keyBytes, byte rounds)
		{
			
			byte[] input = dataBytes;
			byte[] result = null;
			while (rounds > 0)
			{
				result = ComputeAESRound(aesDir, iv, input, keyBytes);
				input = result;
				rounds--;
			}
			return result;

		}

		private static byte[] ComputeAESRound(AESDir aesDir, IV16 iv, byte[] input, byte[] keyBytes)
		{
			var aes = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = iv.Value,
				Key = keyBytes,
				Mode = CipherMode.CBC
			};
			if (aesDir == AESDir.Encrypt)
			{
				using (aes)
				using (var stream = new MemoryStream())
				using (var encryptor = aes.CreateEncryptor())
				using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
				{
					encrypt.Write(input, 0, input.Length);
					encrypt.FlushFinalBlock();
					return stream.ToArray();
				}
			}

			using (aes)
			using (var stream = new MemoryStream())
			using (var decryptor = aes.CreateDecryptor())
			using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
			{
				decrypt.Write(input, 0, input.Length);
				decrypt.FlushFinalBlock();
				return stream.ToArray();
			}
		}

		private enum AESDir
		{
			Encrypt, Decrpyt
		}

		public MAC16 AESDecryptMAC(CipherV2 cipherV2, RandomKey32 randomKey)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (randomKey == null)
				throw new ArgumentNullException("randomKey");

			if (cipherV2.IV16 == null)
				throw new ArgumentException("cipherV2.IV16 must not be null at this point.");

			if (cipherV2.MACCipher == null)
				throw new ArgumentException("cipherV2.MACCipher must not be null at this point.");

			var mac16 = ComputeAES(AESDir.Decrpyt, cipherV2.IV16, cipherV2.MACCipher, randomKey.Value, cipherV2.BWF.Value);
			return new MAC16(mac16);

		}

		public RandomKey32 AESDecryptRandomKeyWithPasswordDerivedKey(CipherV2 cipherV2, PasswordDerivedKey32 passwordDerivedKey)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (cipherV2.IV16 == null)
				throw new ArgumentException("cipherV2.IV16 must not be null at this point.");

			if (cipherV2.RandomKeyCipher == null)
				throw new ArgumentException("cipherV2.RandomKeyCipher must not be null at this point.");

			var randomKey = ComputeAES(AESDir.Decrpyt, cipherV2.IV16, cipherV2.RandomKeyCipher, passwordDerivedKey.Value, cipherV2.BWF.Value);
			return new RandomKey32(randomKey);
		}


		public PaddedData AESDecryptMessage(CipherV2 cipherV2, IV16 iv16, RandomKey32 randomKey)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (iv16 == null)
				throw new ArgumentNullException("iv16");

			if (randomKey == null)
				throw new ArgumentNullException("randomKey");

			var paddedData = ComputeAES(AESDir.Decrpyt, cipherV2.IV16, cipherV2.MessageCipher, randomKey.Value, cipherV2.BWF.Value);
			return new PaddedData(paddedData, cipherV2.Padding);
		}

		public Compressed RemovePadding(PaddedData paddedData)
		{
			if (paddedData == null)
				throw new ArgumentNullException("paddedData");

			var paddingRemoved = new byte[paddedData.DataBytes.Length - paddedData.Padding];

			Buffer.BlockCopy(paddedData.DataBytes, 0, paddingRemoved, 0, paddingRemoved.Length);

			return new Compressed(paddingRemoved);
		}

		public ClearText Decompress(Compressed compressed)
		{
			if (compressed == null)
				throw new ArgumentNullException("compressed");

			var deflate = new Deflate();
			var clearText = deflate.Decompress(compressed.Value, Encoding.UTF8);
			return new ClearText(clearText);
		}

		public string GenerateRandomPassword()
		{
			var passwordBytes = new byte[32];

			using (var rng = new RNGCryptoServiceProvider())
				rng.GetBytes(passwordBytes);

			char[] passwordChars = Base64Encoder.EncodeDataToBase64CharArray(passwordBytes);

			string passwordString = new string(passwordChars).Remove(43).Replace("/", "$");
			var sb = new StringBuilder();

			for (var i = 0; i != passwordString.Length; ++i)
			{
				sb.Append(passwordString[i]);
				var insertSpace = (i + 1) % 5 == 0;
				var insertNewLine = (i + 1) % 25 == 0;
				if (insertNewLine)
					sb.Append(Environment.NewLine);
				else if (insertSpace)
					sb.Append(" ");
			}
			return sb.ToString();
		}

		public byte[] ComputeSHA512(byte[] bytesToHash)
		{
			using (var sha = new SHA512Managed())
			{
				return sha.ComputeHash(bytesToHash);
			}
		}

		public byte[] ComputeSHA256(byte[] bytesToHash)
		{
			using (var sha = new SHA256Managed())
			{
				return sha.ComputeHash(bytesToHash);
			}
		}
	}
}