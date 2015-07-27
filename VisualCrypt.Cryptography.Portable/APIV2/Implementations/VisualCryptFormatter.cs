﻿using System;
using System.IO.Compression;
using System.Text;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Cryptography.Portable.APIV2.Implementations
{
	public class VisualCryptFormatter
	{
		const string VisualCryptSlashText = "VisualCrypt/";

		public VisualCryptText CreateVisualCryptText(CipherV2 cipherV2)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			var visualCryptTextV2Bytes = ByteArrays.Concatenate(
												// len			Sum(len)		Start Index
				new[] {CipherV2.Version},		// 1			1				0
				new[] {cipherV2.BWF.Value},		// 1			2				1
				new[] {cipherV2.Padding},		// 1			3				2
				cipherV2.IV16.Value,			// 16			19				3
				cipherV2.MACCipher,				// 16			35				19
				cipherV2.RandomKeyCipher,		// 32			67				35
				cipherV2.MessageCipher			// len			67 + len		67
				);
			
			

			var visualCryptTextV2Base64 = Base64Encoder.EncodeDataToBase64CharArray(visualCryptTextV2Bytes);

			var sb = new StringBuilder();
			const int breakAfter = 74;
			var charsInLine = 0;

			foreach (var c in VisualCryptSlashText)
			{
				sb.Append(c);
				if (++charsInLine != breakAfter)
					continue;
				sb.Append(new[] {'\r', '\n'});
				charsInLine = 0;
			}

			foreach (var c in visualCryptTextV2Base64)
			{
				sb.Append(c);
				if (++charsInLine != breakAfter)
					continue;
				sb.Append(new[] {'\r', '\n'});
				charsInLine = 0;
			}

			return new VisualCryptText(sb.ToString());
		}

		public CipherV2 DissectVisualCryptText(string visualCryptText)
		{
			if (visualCryptText == null)
				throw new ArgumentNullException("visualCryptText");

			try
			{
				var visualCrypt = visualCryptText.Trim();

				if (!visualCrypt.StartsWith(VisualCryptSlashText, StringComparison.OrdinalIgnoreCase))
					throw new FormatException(
						"The data is not in VisualCrypt/text V2 format (because it does not start with '{0}').".FormatInvariant(
							VisualCryptSlashText));

				var visualCryptTextV2Base64 = visualCrypt.Remove(0, VisualCryptSlashText.Length);

				var visualCryptTextV2Bytes = Base64Encoder.DecodeBase64StringToBinary(visualCryptTextV2Base64);

				//var visualCryptTextV2Bytes = ByteArrays.Concatenate(
				//									// len			Sum(len)		Start Index
				//new[] { CipherV2.Version },		// 1			1				0
				//new[] { cipherV2.BWF.Value },		// 1			2				1
				//new[] { cipherV2.Padding },		// 1			3				2
				//cipherV2.IV16.Value,				// 16			19				3
				//cipherV2.MACCipher,				// 16			35				19
				//cipherV2.RandomKeyCipher,			// 32			67				35
				//cipherV2.MessageCipher			// len			67 + len		67
				//);
			


				var version = visualCryptTextV2Bytes[0];
				var bwf = visualCryptTextV2Bytes[1];
				var padding = visualCryptTextV2Bytes[2];

				if (version != CipherV2.Version)
					throw new FormatException(
						"The data is not in VisualCrypt/text V2 format. Expected a version byte at index 0 of value '2'.");
				
				if (bwf > 31 || bwf < 4)
					throw new FormatException(
						"The data is not in VisualCrypt/text V2 format. The value for the Bcyrpt work factor at index 1 is invalid.");

				
				if (padding > 15)
					throw new FormatException(
						"The data is not in VisualCrypt/text V2 format. The value at the padding byte at index 1 is invalid.");

		
				var cipher = new CipherV2 { Padding = padding , BWF = new BWF(bwf)};


				var iv16 = new byte[16];
				Buffer.BlockCopy(visualCryptTextV2Bytes, 3, iv16, 0, 16);
				cipher.IV16 = new IV16(iv16);

				var macCipher = new byte[16];
				Buffer.BlockCopy(visualCryptTextV2Bytes, 19, macCipher, 0, 16);
				cipher.MACCipher = macCipher;

				var randomKeyCipher = new byte[32];
				Buffer.BlockCopy(visualCryptTextV2Bytes, 35, randomKeyCipher, 0, 32);
				cipher.RandomKeyCipher = randomKeyCipher;

				var cipherBytes = new byte[visualCryptTextV2Bytes.Length - 67];
				Buffer.BlockCopy(visualCryptTextV2Bytes, 67, cipherBytes, 0, cipherBytes.Length);
				cipher.MessageCipher = cipherBytes;

				return cipher;
			}
			catch (Exception e)
			{
				throw new FormatException("Data invalid or truncated. " + e.Message);
			}
		}
	}
}