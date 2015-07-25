using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;

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

		public IV16 GenerateIV(int length)
		{
			var iv = new byte[length];

			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(iv);
				return new IV16(iv);
			}
		}

		public CipherV2 AESEncryptMessage(PaddedData paddedData, AESKey32 aesKey32, IV16 outerIV16)
		{
			if (paddedData == null)
				throw new ArgumentNullException("paddedData");

			if (aesKey32 == null)
				throw new ArgumentNullException("aesKey32");

			if (outerIV16 == null)
				throw new ArgumentNullException("outerIV16");

			var aes = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = outerIV16.Value,
				Key = aesKey32.Value,
				Mode = CipherMode.CBC
			};


			var cipher = new CipherV2 { Padding = paddedData.Padding, IV16 = outerIV16 };

			using (var stream = new MemoryStream())
			using (var encryptor = aes.CreateEncryptor())
			using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
			{
				encrypt.Write(paddedData.DataBytes, 0, paddedData.DataBytes.Length);
				encrypt.FlushFinalBlock();
				cipher.CipherBytes = stream.ToArray();

				return cipher;
			}
		}

	

		public void AESEncryptMessageDigest(CipherV2 cipherv2, MD16 md16, AESKey32 aesKey32)
		{
			if (cipherv2 == null)
				throw new ArgumentNullException("cipherv2");

			if (md16 == null)
				throw new ArgumentNullException("md16");

			if (aesKey32 == null)
				throw new ArgumentNullException("aesKey32");

			var aes = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = cipherv2.IV16.Value,
				Key = aesKey32.Value,
				Mode = CipherMode.CBC
			};


			using (var stream = new MemoryStream())
			using (var encryptor = aes.CreateEncryptor())
			using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
			{
				encrypt.Write(md16.Value, 0, md16.Value.Length);
				encrypt.FlushFinalBlock();
				cipherv2.MD16E = new MD16E(stream.ToArray());
			}
		}

		public MD16 AESDecryptMessageDigest(MD16E md16E, IV16 iv16, AESKey32 aesKey32)
		{
			if (md16E == null)
				throw new ArgumentNullException("md16E");

			if (iv16 == null)
				throw new ArgumentNullException("iv16");

			if (aesKey32 == null)
				throw new ArgumentNullException("aesKey32");

			var aes = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = iv16.Value,
				Key = aesKey32.Value,
				Mode = CipherMode.CBC
			};

			using (var stream = new MemoryStream())
			using (var decryptor = aes.CreateDecryptor())
			using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
			{
				decrypt.Write(md16E.Value, 0, md16E.Value.Length);
				decrypt.FlushFinalBlock();
				return new MD16(stream.ToArray());
			}
		}

		public PaddedData AESDecryptMessage(CipherV2 cipherV2, IV16 iv16, AESKey32 aesKey32)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (iv16 == null)
				throw new ArgumentNullException("iv16");

			if (aesKey32 == null)
				throw new ArgumentNullException("aesKey32");

			var aes = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = iv16.Value,
				Key = aesKey32.Value,
				Mode = CipherMode.CBC
			};

			using (var stream = new MemoryStream())
			using (var decryptor = aes.CreateDecryptor())
			using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
			{
				decrypt.Write(cipherV2.CipherBytes, 0, cipherV2.CipherBytes.Length);
				decrypt.FlushFinalBlock();
				return new PaddedData(stream.ToArray(), cipherV2.Padding);
			}
		}

		public PaddedData AESDecryptMessage2(CipherV2 cipherV2, IV16 outerIV16, AESKey32 aesKey32)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (outerIV16 == null)
				throw new ArgumentNullException("outerIV16");

			if (aesKey32 == null)
				throw new ArgumentNullException("aesKey32");

			var outerAES = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = outerIV16.Value,
				Key = aesKey32.Value,
				Mode = CipherMode.CBC
			};

			byte[] innerCipherAndIV;

			using (var stream = new MemoryStream())
			using (var decryptor = outerAES.CreateDecryptor())
			using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
			{
				decrypt.Write(cipherV2.CipherBytes, 0, cipherV2.CipherBytes.Length);
				decrypt.FlushFinalBlock();
				innerCipherAndIV = stream.ToArray();
				
			}
			var innerCipher = new byte[innerCipherAndIV.Length - 16];
			var innerIV = new byte[16];
			Buffer.BlockCopy(innerCipherAndIV,0,innerCipher,0,innerCipher.Length);
			Buffer.BlockCopy(innerCipherAndIV,innerCipher.Length,innerIV,0,innerIV.Length);

			var innerAES = new AesManaged
			{
				KeySize = 256,
				BlockSize = 128,
				Padding = PaddingMode.None,
				IV = innerIV,
				Key = aesKey32.Value,
				Mode = CipherMode.CBC
			};

			using (var stream = new MemoryStream())
			using (var decryptor = innerAES.CreateDecryptor())
			using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
			{
				decrypt.Write(innerCipher, 0, innerCipher.Length);
				decrypt.FlushFinalBlock();
				return new PaddedData(stream.ToArray(), cipherV2.Padding);

			}

			
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
	}
}