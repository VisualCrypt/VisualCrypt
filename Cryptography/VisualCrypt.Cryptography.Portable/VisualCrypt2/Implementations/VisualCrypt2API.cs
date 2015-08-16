using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;
using VisualCrypt.Language;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations
{
	public class VisualCrypt2API : IVisualCrypt2API
	{
		readonly IPlatform _platform;
	    readonly VisualCryptAPI2Internal _internal;

		public VisualCrypt2API(IPlatform platform)
		{
			if (platform == null)
				throw new ArgumentNullException("platform", "The platform-specific API part is mandantory.");

			_platform = platform;
            _internal = new VisualCryptAPI2Internal(_platform);
		}


		public Response<SHA512PW64> CreateSHA512PW64(string unpruned)
		{
			var response = new Response<SHA512PW64>();

			try
			{
				if (unpruned == null)
				{
					response.SetError("Argument null: 'unpruned'");
					return response;
				}

				var prunedPasswordResponse = PrunePassword(unpruned);
				if (!prunedPasswordResponse.IsSuccess)
				{
					response.SetError(prunedPasswordResponse.Error);
					return response;
				}

				var utf16LEBytes = Encoding.Unicode.GetBytes(prunedPasswordResponse.Result.Text);

				var sha512 = _platform.ComputeSHA512(utf16LEBytes);

				response.Result = new SHA512PW64(sha512);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		public Response<CipherV2> Encrypt(Cleartext cleartext, SHA512PW64 sha512PW64, RoundsExponent roundsExponent,
			LongRunningOperationContext context)
		{
			var response = new Response<CipherV2>();

			try
			{
				Guard.NotNull(new object[] { cleartext, sha512PW64, roundsExponent, context });

				Compressed compressed = _internal.Compress(cleartext);

				PaddedData paddedData = _internal.ApplyRandomPadding(compressed);

				IV16 iv = new IV16(_platform.GenerateRandomBytes(16));

				PasswordDerivedKey32 passwordDerivedKey = CreatePasswordDerivedKey(iv, sha512PW64, roundsExponent, context);

				RandomKey32 randomKey = new RandomKey32(_platform.GenerateRandomBytes(32));

				var cipherV2 = new CipherV2 { RoundsExponent = roundsExponent, IV16 = iv };
				_internal.AESEncryptRandomKeyWithPasswordDerivedKey(passwordDerivedKey, randomKey, cipherV2, context);

				_internal.AESEncryptMessageWithRandomKey(paddedData, randomKey, cipherV2, context);

				MAC16 mac = CreateMAC(cipherV2, context);

				_internal.AESEncryptMACWithRandomKey(cipherV2, mac, randomKey, context);

				response.Result = cipherV2;
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		static MAC16 CreateMAC(CipherV2 cipherV2, LongRunningOperationContext context)
		{
			Guard.NotNull(new object[] { cipherV2, context });

			// Create the MAC only for items that, while decrypting, have not been used up to this point but do include the version.
			var securables = ByteArrays.Concatenate(cipherV2.MessageCipher.GetBytes(), new[] { cipherV2.Padding.ByteValue },
				new[] { CipherV2.Version });

			context.EncryptionProgress.Message = Loc.Strings.encProgr_CalculatingMAC;
			BCrypt24 slowMAC = BCrypt.CreateHash(cipherV2.IV16, securables, cipherV2.RoundsExponent.Value, context);

			// See e.g. http://csrc.nist.gov/publications/fips/fips180-4/fips-180-4.pdf Chapter 7 for hash truncation.
			var truncatedMAC = new byte[16];
			Buffer.BlockCopy(slowMAC.GetBytes(), 0, truncatedMAC, 0, 16);
			return new MAC16(truncatedMAC);
		}


		PasswordDerivedKey32 CreatePasswordDerivedKey(IV16 iv, SHA512PW64 sha512PW64, RoundsExponent roundsExponent,
			LongRunningOperationContext context)
		{
			Guard.NotNull(new object[] { iv, sha512PW64, roundsExponent, context });

			var leftSHA512 = new byte[32];
			var rightSHA512 = new byte[32];
			Buffer.BlockCopy(sha512PW64.GetBytes(), 0, leftSHA512, 0, 32);
			Buffer.BlockCopy(sha512PW64.GetBytes(), 32, rightSHA512, 0, 32);

			context.EncryptionProgress.Message = Loc.Strings.encProgr_ProcessingKey;

			// Compute the left side on a ThreadPool thread
			var task = Task.Run(() => BCrypt.CreateHash(iv, leftSHA512, roundsExponent.Value, context));

			// Compute the right side after dispatching the work for the right side
			BCrypt24 rightBCrypt = BCrypt.CreateHash(iv, rightSHA512, roundsExponent.Value, context);

			// Wait for the left side result
			task.Wait(context.CancellationToken);

			// Use the results
			var combinedHashes = ByteArrays.Concatenate(sha512PW64.GetBytes(), task.Result.GetBytes(), rightBCrypt.GetBytes());
			Debug.Assert(combinedHashes.Length == 64 + 24 + 24);

			var condensedHash = _platform.ComputeSHA256(combinedHashes);
			return new PasswordDerivedKey32(condensedHash);
		}


		public Response<VisualCryptText> EncodeToVisualCryptText(CipherV2 cipherV2)
		{
			var response = new Response<VisualCryptText>();

			try
			{
				Guard.NotNull(cipherV2);

				response.Result = VisualCrypt2Formatter.CreateVisualCryptText(cipherV2);
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
			var response = new Response<CipherV2>();

			try
			{
				Guard.NotNull(visualCryptText);
			    var pruned = visualCryptText.FilterWhitespaceAndControlCharacters();
				response.Result = VisualCrypt2Formatter.DissectVisualCryptText(pruned);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		public Response<Cleartext> Decrypt(CipherV2 cipherV2, SHA512PW64 sha512PW64, LongRunningOperationContext context)
		{
			var response = new Response<Cleartext>();

			try
			{
				Guard.NotNull(new object[] { cipherV2, sha512PW64, context });

				PasswordDerivedKey32 passwordDerivedKey = CreatePasswordDerivedKey(cipherV2.IV16, sha512PW64, cipherV2.RoundsExponent,
					context);

				RandomKey32 randomKey = _internal.AESDecryptRandomKeyWithPasswordDerivedKey(cipherV2, passwordDerivedKey, context);

				MAC16 decryptedMAC = _internal.AESDecryptMAC(cipherV2, randomKey, context);

				MAC16 actualMAC = CreateMAC(cipherV2, context);

				if (!actualMAC.GetBytes().SequenceEqual(decryptedMAC.GetBytes()))
				{
					response.SetError(Loc.Strings.msgPasswordError);
					return response;
				}


				PaddedData paddedData = _internal.AESDecryptMessage(cipherV2, cipherV2.IV16, randomKey, context);

				Compressed compressed = _internal.RemovePadding(paddedData);

				Cleartext cleartext = _internal.Decompress(compressed);

				response.Result = cleartext;
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		public Response<string, Encoding> GetStringFromFile(byte[] data,
			Encoding platformDefaultEncoding)
		{
			var response = new Response<string, Encoding>();

			try
			{
				Guard.NotNull(data); // platformDefaultEncoding is allowed to be null

				Encoding encoding;
				var decodeFileResult = FileContentsDetection.GetTextDetectingEncoding(data, out encoding, platformDefaultEncoding);
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
				response.Result = _internal.GenerateRandomPassword();
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
		/// <param name="unprunedPassword">The password string obtained from textBox.Text.</param>
		/// <returns>The sanitized UTF-16 password string, the bytes of which are used as input for the password hash function.</returns>
		/// <see cref="http://www.unicode.org/Public/UNIDATA/UnicodeData.txt"/>
		public Response<PrunedPassword> PrunePassword(string unprunedPassword)
		{
			var response = new Response<PrunedPassword>();
			try
			{
				Guard.NotNull(unprunedPassword);

				// from msdn: White-space characters are defined by the Unicode standard. 
				// The Trim() method removes any leading and trailing characters that produce 
				// a return value of true when they are passed to the Char.IsWhiteSpace method.
				string sanitized =
					unprunedPassword
						.Trim()
						.FilterNonWhitespaceControlCharacters()
						.CondenseWhiteSpace();

				response.Result = new PrunedPassword(sanitized);
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
        public static string FilterWhitespaceAndControlCharacters(this string visualCryptBase64)
        {
            Guard.NotNull(visualCryptBase64);

            var charArray =
                visualCryptBase64
                    .ToCharArray()
                    .Where(c => !char.IsControl(c) && !char.IsWhiteSpace(c))
                    .ToArray();

            return new string(charArray);
        }

        public static string FilterNonWhitespaceControlCharacters(this string unprunedPassword)
		{
			Guard.NotNull(unprunedPassword);

			var charArray =
				unprunedPassword
					.ToCharArray()
					.Where(c => !(char.IsControl(c) && !char.IsWhiteSpace(c)))
					.ToArray();

			return new string(charArray);
		}

		public static string CondenseWhiteSpace(this string unpruned)
		{
			Guard.NotNull(unpruned);

			return
				unpruned
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