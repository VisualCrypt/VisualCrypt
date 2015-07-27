using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Cryptography.Portable.APIV2.Implementations
{
	public class VisualCryptAPIV2 : IVisualCryptAPIV2
	{
		readonly ICoreAPIV2 _coreAPI;

		public VisualCryptAPIV2(ICoreAPIV2 coreAPI)
		{
			if (coreAPI == null)
				throw new ArgumentNullException("coreAPI", "The platform-specific API part is mandantory.");

			_coreAPI = coreAPI;
		}

		public Response<SHA512PW64> CreateSHA512PW64(string unsanitizedUTF16LEPassword)
		{
			var response = new Response<SHA512PW64>();

			try
			{
				if (unsanitizedUTF16LEPassword == null)
				{
					response.SetError("Argument null: 'unsanitizedUTF16LEPassword'");
					return response;
				}

				var sanitizedPasswordResponse = SanitizePassword(unsanitizedUTF16LEPassword);
				if (!sanitizedPasswordResponse.IsSuccess)
				{
					response.SetError(sanitizedPasswordResponse.Error);
					return response;
				}

				var utf16LEBytes = Encoding.Unicode.GetBytes(sanitizedPasswordResponse.Result.Value);

				var sha512 = _coreAPI.ComputeSHA512(utf16LEBytes);

				response.Result = new SHA512PW64(sha512);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}



		public Response<CipherV2> Encrypt(ClearText clearText, SHA512PW64 sha512PW64, BWF bwf, IProgress<int> progress, CancellationToken cToken)

		{
			if (clearText == null)
				return new Response<CipherV2>("Argument null: clearText");

			if (sha512PW64 == null)
				return new Response<CipherV2>("Argument null: SHA512PW64");

			var response = new Response<CipherV2>();

			try
			{
				Compressed compressed = _coreAPI.Compress(clearText);

				PaddedData paddedData = _coreAPI.ApplyRandomPadding(compressed);


				IV16 iv = new IV16(_coreAPI.GenerateRandomBytes(16));

				PasswordDerivedKey32 passwordDerivedKey = CreatePasswordDerivedKey(iv, sha512PW64, bwf, progress, cToken);

				RandomKey32 randomKey = new RandomKey32(_coreAPI.GenerateRandomBytes(32));

				var cipherV2 = new CipherV2 { BWF = bwf, IV16 = iv };
				_coreAPI.AESEncryptRandomKeyWithPasswordDerivedKey(passwordDerivedKey, randomKey, cipherV2);

				_coreAPI.AESEncryptMessageWithRandomKey(paddedData, randomKey, cipherV2);


				MAC16 mac = CreateMAC(cipherV2, progress, cToken);

				_coreAPI.AESEncryptMACWithRandomKey(cipherV2, mac, randomKey);

				response.Result = cipherV2;
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		MAC16 CreateMAC(CipherV2 cipherV2, IProgress<int> progress, CancellationToken cToken)
		{
			if (cipherV2 == null)
				throw new ArgumentNullException("cipherV2");

			if (progress == null)
				throw new ArgumentNullException("progress");

			if (cToken == null)
				throw new ArgumentNullException("cToken");

			if (cipherV2.MessageCipher == null)
				throw new ArgumentException("cipherV2.MessageCipher must not be null at this point.");

			if (cipherV2.IV16 == null)
				throw new ArgumentException("cipherV2.IV16 must not be null at this point.");

			// Create the MAC only for items that, while decrypting, have not been used up to this point but do include the version.
			var securables = ByteArrays.Concatenate(cipherV2.MessageCipher, new[] { cipherV2.Padding }, new[] { CipherV2.Version });

			BCrypt24 slowMAC = BCrypt.CreateHash(cipherV2.IV16, securables, cipherV2.BWF.Value, progress, cToken);

			// See e.g. http://csrc.nist.gov/publications/fips/fips180-4/fips-180-4.pdf Chapter 7 for hash truncation.
			var truncatedMAC = new byte[16];
			Buffer.BlockCopy(slowMAC.Value, 0, truncatedMAC, 0, 16);
			return new MAC16(truncatedMAC);
		}


		PasswordDerivedKey32 CreatePasswordDerivedKey(IV16 iv, SHA512PW64 sha512PW64, BWF bwf, IProgress<int> progress, CancellationToken cToken)
		{
			var leftSHA512 = new byte[32];
			var rightSHA512 = new byte[32];
			Buffer.BlockCopy(sha512PW64.Value, 0, leftSHA512, 0, 32);
			Buffer.BlockCopy(sha512PW64.Value, 32, rightSHA512, 0, 32);

			BCrypt24 leftBCrypt = BCrypt.CreateHash(iv, leftSHA512, bwf.Value, progress, cToken);
			BCrypt24 rightBCrypt = BCrypt.CreateHash(iv, rightSHA512, bwf.Value, progress, cToken);

			var combinedHashes = ByteArrays.Concatenate(sha512PW64.Value, leftBCrypt.Value, rightBCrypt.Value);
			Debug.Assert(combinedHashes.Length == 64 + 24 + 24);

			var condensedHash = _coreAPI.ComputeSHA256(combinedHashes);
			return new PasswordDerivedKey32(condensedHash);

		}


		public Response<VisualCryptText> EncodeToVisualCryptText(CipherV2 cipherV2)
		{
			if (cipherV2 == null)
				return new Response<VisualCryptText>("Argument null: cipherV2");

			var response = new Response<VisualCryptText>();

			try
			{
				var formatter = new VisualCryptFormatter();
				response.Result = formatter.CreateVisualCryptText(cipherV2);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response<CipherV2> TryDecodeVisualCryptText(string visualCryptText)
		{
			if (visualCryptText == null)
				return new Response<CipherV2>("Argument null: visualCryptText");

			var response = new Response<CipherV2>();

			try
			{
				var formatter = new VisualCryptFormatter();
				response.Result = formatter.DissectVisualCryptText(visualCryptText);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		public Response<ClearText> Decrypt(CipherV2 cipherV2, SHA512PW64 sha512PW64, IProgress<int> progress, CancellationToken cToken)
		{
			var response = new Response<ClearText>();

			try
			{
				PasswordDerivedKey32 passwordDerivedKey = CreatePasswordDerivedKey(cipherV2.IV16, sha512PW64, cipherV2.BWF, progress, cToken);

				RandomKey32 randomKey = _coreAPI.AESDecryptRandomKeyWithPasswordDerivedKey(cipherV2, passwordDerivedKey);

				MAC16 decryptedMAC = _coreAPI.AESDecryptMAC(cipherV2, randomKey);

				MAC16 actualMAC = CreateMAC(cipherV2, progress, cToken);

				if (!actualMAC.Value.SequenceEqual(decryptedMAC.Value))
				{
					response.SetError("The password is wrong or the data has been corrupted.");
					return response;
				}


				PaddedData paddedData = _coreAPI.AESDecryptMessage(cipherV2, cipherV2.IV16, randomKey);

				Compressed compressed = _coreAPI.RemovePadding(paddedData);

				ClearText clearText = _coreAPI.Decompress(compressed);

				response.Result = clearText;
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}



		public Response<string, Encoding> GetStringFromFileBytes(byte[] rawBytesFromFile,
			Encoding platformDefaultEncoding = null)
		{
			var response = new Response<string, Encoding>();

			if (rawBytesFromFile == null)
			{
				response.SetError("Argument null: ' rawBytesFromFile'");
				return response;
			}

			try
			{
				var detection = new FileContentsDetection();
				Encoding encoding;
				var decodeFileResult = detection.GetTextDetectingEncoding(rawBytesFromFile, out encoding, platformDefaultEncoding);
				if (decodeFileResult != null)
				{
					response.Result = decodeFileResult;
					response.Result2 = encoding;
					response.SetSuccess();
				}
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		public Response<string> GenerateRandomPassword()
		{
			var response = new Response<string>();
			try
			{
				response.Result = _coreAPI.GenerateRandomPassword();
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		/// <summary>
		/// Removes all leading and trailing Unicode whitespace characters and replaces the remaining whitespace characters
		/// with u\0020 space characters. Adjacent whitespace is condensed to a single u\0020 character. Other Unicode
		/// control characters are stripped completely.
		/// The control characters are specifically the Unicode values U+0000 to U+001F and U+007F to U+009F;
		/// whitespace characters as defined by Char.IsWhiteSpace in .net 4.5.
		/// </summary>
		/// <param name="unsanitizedPassword">The password string obtained from textBox.Text.</param>
		/// <returns>The sanitized UTF-16 password string, the bytes of which are used as input for the password hash function.</returns>
		/// <see cref="http://www.unicode.org/Public/UNIDATA/UnicodeData.txt"/>
		public Response<SanitizedPassword> SanitizePassword(string unsanitizedPassword)
		{
			var response = new Response<SanitizedPassword>();
			try
			{
				if (unsanitizedPassword == null)
					throw new ArgumentNullException("unsanitizedPassword");
				// from msdn: White-space characters are defined by the Unicode standard. 
				// The Trim() method removes any leading and trailing characters that produce 
				// a return value of true when they are passed to the Char.IsWhiteSpace method.
				string sanitized =
					unsanitizedPassword
						.Trim()
						.FilterNonWhitespaceControlCharacters()
						.CondenseWhiteSpace();

				response.Result = new SanitizedPassword(sanitized);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}
	}

	internal static class SanitizationStringExtensions
	{
		public static string FilterNonWhitespaceControlCharacters(this string unsanitized)
		{
			var charArray =
				unsanitized
					.ToCharArray()
					.Where(c => !(char.IsControl(c) && !char.IsWhiteSpace(c)))
					.ToArray();

			return new string(charArray);
		}

		public static string CondenseWhiteSpace(this string unsanitized)
		{
			return
				unsanitized
					.ToCharArray()
					.Select(c => char.IsWhiteSpace(c) ? ' ' : c)
					.Aggregate(new StringBuilder(),
						(sb, next) =>
						{
							if (sb.Length == 0)
								return sb.Append(next);
							var previous = sb[sb.Length - 1];
							if (previous == ' ' && next == ' ')
								return sb;
							return sb.Append(next);
						})
					.ToString();
		}
	}
}