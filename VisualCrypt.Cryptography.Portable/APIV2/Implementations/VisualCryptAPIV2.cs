﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;

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
		

		public Response<SHA256PW32> CreateSHA256PW32(string unsanitizedUTF16LEPassword)
		{
			var response = new Response<SHA256PW32>();

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
				using (var sha = new SHA256ManagedMono())
				{
					var hash = sha.ComputeHash(utf16LEBytes);
					response.Result = new SHA256PW32(hash);
					response.SetSuccess();
				}
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response<CipherV2> Encrypt(ClearText clearText, SHA256PW32 sha256PW32, BWF bwf, IProgress<int> progress, CancellationToken cToken)
		{
			if (clearText == null)
				return new Response<CipherV2>("Argument null: clearText");

			if (sha256PW32 == null)
				return new Response<CipherV2>("Argument null: sha256PW32");

			var response = new Response<CipherV2>();

			try
			{
				Compressed compressed = _coreAPI.Compress(clearText);

				PaddedData paddedData = _coreAPI.ApplyRandomPadding(compressed);

				IV16 iv16 = _coreAPI.GenerateIV(16);

				BCrypt24 bcrypt24 = BCrypt.CreateHash(iv16, sha256PW32, bwf.Value, progress, cToken);

				AESKey32 aesKey32;
				using (var sha = new SHA256ManagedMono())
				{
					var hash = sha.ComputeHash(bcrypt24.Value);
					aesKey32 = new AESKey32(hash);
				}

				CipherV2 cipherV2 = _coreAPI.AESEncryptMessage(paddedData, aesKey32, iv16);
				cipherV2.BWF = bwf.Value;

				MD32 md32;
				using (var sha = new SHA256ManagedMono())
				{
					var hash = sha.ComputeHash(cipherV2.CipherBytes);
					md32 = new MD32(hash);
				}

				var first16 = new byte[16];
				Buffer.BlockCopy(md32.Value, 0, first16, 0, 16);
				MD16 md16 = new MD16(first16);

				_coreAPI.AESEncryptMessageDigest(cipherV2, md16, aesKey32);

				response.Result = cipherV2;
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
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

		public Response<ClearText> Decrypt(CipherV2 cipherV2, SHA256PW32 sha256PW32, IProgress<int> progress, CancellationToken cToken)
		{
			var response = new Response<ClearText>();

			try
			{
				BCrypt24 bcrypt24 = BCrypt.CreateHash(cipherV2.IV16, sha256PW32, cipherV2.BWF, progress, cToken);

				AESKey32 aesKey32;
				using (var sha = new SHA256ManagedMono())
				{
					var hash = sha.ComputeHash(bcrypt24.Value);
					aesKey32 = new AESKey32(hash);
				}

				MD16 md16a = _coreAPI.AESDecryptMessageDigest(cipherV2.MD16E, cipherV2.IV16, aesKey32);

				MD32 md32;
				using (var sha = new SHA256ManagedMono())
				{
					var hash = sha.ComputeHash(cipherV2.CipherBytes);
					md32 = new MD32(hash);
				}

				var first16 = new byte[16];
				Buffer.BlockCopy(md32.Value, 0, first16, 0, 16);
				MD16 md16b = new MD16(first16);

				if (!md16a.Value.SequenceEqual(md16b.Value))
				{
					response.SetError("The password is wrong or the data has been corrupted.");
					return response;
				}

				PaddedData paddedData = _coreAPI.AESDecryptMessage(cipherV2, cipherV2.IV16, aesKey32);

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

		public Response<SanitizedPassword> SanitizePassword(string unsanitizedPassword)
		{
			var response = new Response<SanitizedPassword>();
			try
			{
				if (unsanitizedPassword == null)
					throw new ArgumentNullException("unsanitizedPassword");

				string trimmedPassword = unsanitizedPassword.Trim();
				char[] trimmedPasswordChars = trimmedPassword.ToCharArray();
				const char chTab = '\t';
				const char chLineFeed = '\r';
				const char chCarriageReturn = '\n';

				var sb = new StringBuilder();
				foreach (var ch in trimmedPasswordChars)
				{
					if (ch != chTab && ch != chCarriageReturn && ch != chLineFeed)
						sb.Append(ch);
				}
				response.Result = new SanitizedPassword(sb.ToString());
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}
	}
}