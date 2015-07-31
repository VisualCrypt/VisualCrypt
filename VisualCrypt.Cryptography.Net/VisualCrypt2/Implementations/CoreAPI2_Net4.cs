﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;

namespace VisualCrypt.Cryptography.Net.VisualCrypt2.Implementations
{
	public class CoreAPI2_Net4 : ICoreAPI2
	{
		public Compressed Compress(ClearText clearText)
		{
			if (clearText == null)
				throw new ArgumentNullException("clearText");

			byte[] compressed = Deflate.Compress(clearText.Text, Encoding.UTF8);

			return new Compressed(compressed);
		}

		public PaddedData ApplyRandomPadding(Compressed compressed)
		{
			if (compressed == null)
				throw new ArgumentNullException("compressed");

			int requiredPadding;
			if (compressed.GetBytes().Length % 16 == 0)
				requiredPadding = 0;
			else
				requiredPadding = 16 - compressed.GetBytes().Length % 16;

			var paddingBytes = new byte[requiredPadding];

			using (var rng = new RNGCryptoServiceProvider())
				rng.GetBytes(paddingBytes);

			var paddedDataBytes = new byte[compressed.GetBytes().Length + requiredPadding];
			Buffer.BlockCopy(compressed.GetBytes(), 0, paddedDataBytes, 0, compressed.GetBytes().Length);
			Buffer.BlockCopy(paddingBytes, 0, paddedDataBytes, compressed.GetBytes().Length, paddingBytes.Length);

			return new PaddedData(paddedDataBytes, new PlaintextPadding(requiredPadding));
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

			cipherV2.RandomKeyCipher32 = new RandomKeyCipher32(ComputeAES(AESDir.Encrypt, cipherV2.IV16, randomKey.GetBytes(), passwordDerivedKey.GetBytes(), cipherV2.RoundsExponent.Value));
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

			cipherV2.Padding = paddedData.PlaintextPadding;

			cipherV2.MessageCipher = new MessageCipher(ComputeAES(AESDir.Encrypt, cipherV2.IV16, paddedData.GetBytes(), randomKey.GetBytes(), cipherV2.RoundsExponent.Value));

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

			cipherV2.MACCipher16 = new MACCipher16(ComputeAES(AESDir.Encrypt, cipherV2.IV16, mac.GetBytes(), randomKey.GetBytes(), cipherV2.RoundsExponent.Value));
		}

		
		byte[] ComputeAES(AESDir aesDir, IV16 iv, byte[] dataBytes, byte[] keyBytes, byte roundsExp)
		{
			var rounds = 1u << roundsExp;

			if (_ivCache == null || _ivCache.Item1.GetBytes().SequenceEqual(iv.GetBytes()) == false)
				_ivCache = CreateIVTable(iv, rounds);

			byte[] inputData = dataBytes;
			byte[] aesResult = null;
			while (rounds > 0)
			{
				 IV16 currentIV = 
					 aesDir == AESDir.Encrypt
						? _ivCache.Item2[rounds - 1] 
						: _ivCache.Item2[_ivCache.Item2.Length - rounds];

				aesResult = ComputeAESRound(aesDir, currentIV.GetBytes(), inputData, keyBytes);
				inputData = aesResult;
				rounds--;
			}
			return aesResult;

		}

		Tuple<IV16, IV16[]> _ivCache;
		Tuple<IV16, IV16[]> CreateIVTable(IV16 iv, uint rounds)
		{
			var ivRounds = rounds;
			var ivTable = new IV16[rounds];
			byte[] ivInput = iv.GetBytes();
			while (ivRounds > 0)
			{

				ivTable[ivTable.Length - ivRounds] = new IV16(ivInput);
					
				ivInput = 	ComputeSHA256(ivInput).Take(16).ToArray();

				ivRounds = ivRounds - 1;
			}
			return new Tuple<IV16, IV16[]>(iv, ivTable);
		}


		private static byte[] ComputeAESRound(AESDir aesDir, byte[] iv, byte[] input, byte[] keyBytes)
		{
			var aes = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = iv,
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

			if (cipherV2.MACCipher16 == null)
				throw new ArgumentException("cipherV2.MACCipher16 must not be null at this point.");

			var mac16 = ComputeAES(AESDir.Decrpyt, cipherV2.IV16, cipherV2.MACCipher16.GetBytes(), randomKey.GetBytes(), cipherV2.RoundsExponent.Value);
			return new MAC16(mac16);

		}

		public RandomKey32 AESDecryptRandomKeyWithPasswordDerivedKey(CipherV2 cipherV2, PasswordDerivedKey32 passwordDerivedKey)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if(passwordDerivedKey == null)
				throw new ArgumentNullException("passwordDerivedKey");

			if (cipherV2.IV16 == null)
				throw new ArgumentException("cipherV2.IV16 must not be null at this point.");

			if (cipherV2.RandomKeyCipher32 == null)
				throw new ArgumentException("cipherV2.RandomKeyCipher32 must not be null at this point.");

			var randomKey = ComputeAES(AESDir.Decrpyt, cipherV2.IV16, cipherV2.RandomKeyCipher32.GetBytes(), passwordDerivedKey.GetBytes(), cipherV2.RoundsExponent.Value);
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

			var paddedData = ComputeAES(AESDir.Decrpyt, cipherV2.IV16, cipherV2.MessageCipher.GetBytes(), randomKey.GetBytes(), cipherV2.RoundsExponent.Value);
			return new PaddedData(paddedData, cipherV2.Padding);
		}

		public Compressed RemovePadding(PaddedData paddedData)
		{
			if (paddedData == null)
				throw new ArgumentNullException("paddedData");

			var paddingRemoved = new byte[paddedData.GetBytes().Length - paddedData.PlaintextPadding.ByteValue];

			Buffer.BlockCopy(paddedData.GetBytes(), 0, paddingRemoved, 0, paddingRemoved.Length);

			return new Compressed(paddingRemoved);
		}

		public ClearText Decompress(Compressed compressed)
		{
			if (compressed == null)
				throw new ArgumentNullException("compressed");

			var clearText = Deflate.Decompress(compressed.GetBytes(), Encoding.UTF8);
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

		public byte[] ComputeSHA512(byte[] data)
		{
			using (var sha = new SHA512Managed())
			{
				return sha.ComputeHash(data);
			}
		}

		public byte[] ComputeSHA256(byte[] data)
		{
			using (var sha = new SHA256Managed())
			{
				return sha.ComputeHash(data);
			}
		}
	}
}