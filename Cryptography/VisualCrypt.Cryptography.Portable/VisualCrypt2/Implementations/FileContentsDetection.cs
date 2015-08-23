﻿using System.Globalization;
using System.Linq;
using System.Text;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.VisualCrypt2.Implementations
{
    public static class FileContentsDetection
	{
		/// <summary>
		/// Detects the contents of an unknown file that is being loaded for display purposes.
		/// - If VisualCrypt is detected, this is just from the prefix, does not mean it's valid or what version.
		/// </summary>
		internal static string GetTextDetectingEncoding(byte[] rawBytesFromFile, out Encoding appliedEncoding,
			Encoding optionalPlatformDefaultEncoding)
		{
			Guard.NotNull(new object[] { rawBytesFromFile });  // optionalPlatformDefaultEncoding is allowed to be null

			var byteCount = rawBytesFromFile.Length;

			// If the file is empty, just return string.Empty and set ContentKind to PlainText.
			if (byteCount == 0)
			{
				appliedEncoding = null;
				return string.Empty;
			}

			// Try by looking for a Unicode signature/BOM...
			// ...if found, return fault-tolerant decoder and use it.
			var signatureUnicodeEncoding = TryGetEncodingBySignatureDetection(rawBytesFromFile, false);
			if (signatureUnicodeEncoding != null)
			{
				appliedEncoding = signatureUnicodeEncoding;
				return signatureUnicodeEncoding.GetString(rawBytesFromFile, 0, byteCount);
			}

			// Try if UTF8 decoding would work without errors
			var strictUTF8Encoding = new UTF8Encoding(false /* signature */, true /* trow on errors */);
			try
			{
				appliedEncoding = strictUTF8Encoding;
				return strictUTF8Encoding.GetString(rawBytesFromFile, 0, byteCount);
			}
			catch (DecoderFallbackException)
			{
			}

			// If we are here it's a very likely a single byte encoding but not valid UTF8.
			// Before we continue, let's make a simple check whether it's more likely a binary than text.
			decimal controlCharPercent = GetIllegalCharacterPercent(rawBytesFromFile);
			if (controlCharPercent >= 2)
			{
				// yes, this is probaby no text at all, encode it to displayable hex numbers to show the user 'something'.
				appliedEncoding = null;
				// null has the special meaning it's Hex View (can't create a custom encoding with EncodingName property in portable class library)
				return rawBytesFromFile.ToHexView();
			}

			// If we are e.g. on Desktop, try injected default encoding.
			if (optionalPlatformDefaultEncoding != null)
			{
				appliedEncoding = optionalPlatformDefaultEncoding;
				return optionalPlatformDefaultEncoding.GetString(rawBytesFromFile, 0, byteCount);
			}

			var encodingString = GetUserCultureBasedEncoding();
			try
			{
				var cultureBasedEncoding = Encoding.GetEncoding(encodingString);
				appliedEncoding = cultureBasedEncoding;
				return cultureBasedEncoding.GetString(rawBytesFromFile, 0, byteCount);
			}
			catch (DecoderFallbackException)
			{
				var tolerantUTF8Encoding = new UTF8Encoding(false, false);
				appliedEncoding = tolerantUTF8Encoding;
				return tolerantUTF8Encoding.GetString(rawBytesFromFile, 0, byteCount);
			}
		}

		// http://bytes.com/topic/c-sharp/answers/521848-streamreader-system-text-encoding-german-character
		static string GetUserCultureBasedEncoding()
		{
			var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			string[] iso1 =
			{
				"af", "sq", "eu", "da", "de", "en", "et", "fo", "fi", "fr", "ga", "is", "it", "ca", "nl", "no",
				"pt", "rm", "gd", "sv", "es", "sw", "wa"
			};
			if (iso1.Contains(culture))
				return "iso-8859-1";

			string[] iso2 =
			{
				"bs", "hr", "pl", "sr", "sk", "sl", "cs", "hu"
			};
			if (iso2.Contains(culture))
				return "iso-8859-2";

			string[] iso5 =
			{
				"ru", "ua"
			};
			if (iso5.Contains(culture))
				return "iso-8859-5";
			return null;
		}

		/// <summary>
		/// Returns the percentage (0-100) of illegal chars,
		/// assuming we know it's a single byte ISO-8859-x or Windows-125x encoding
		/// or UTF8 or some other encoding that uses traditional control characters.
		/// </summary>
		static decimal GetIllegalCharacterPercent(byte[] rawBytesFromFile)
		{
			decimal totalChars = rawBytesFromFile.Length;
			decimal illegalCharCount = 0;
			foreach (byte t in rawBytesFromFile)
			{
				if (IsIllegalChar(t))
					illegalCharCount++;
			}

			return illegalCharCount / totalChars * 100;
		}

		/// <summary>
		/// We declare a char illegal if it's lower 0x20 but not one of the following:
		/// 00 NUL \0 Null character 
		/// 09 HT \t Horizontal Tab
		/// 0A LF \n Line feed 
		/// 0D CR \r Carriage return
		/// </summary>
		static bool IsIllegalChar(byte ch)
		{
			if (ch < 0x20)
			{
				if (ch != 0x00 && ch != 0x09 && ch != 0x0A && ch != 0x0D)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Detects the following encodings by searching for a Unicode signature:
		/// FF FE           Unicode 1200
		/// FF FE 00 00     Unicode (UTF-32) 12000
		/// FE FF           Unicode (Big-Endian) 1201
		/// EF BB BF        Unicode (UTF-8) 65001
		/// 00 00 FE FF     Unicode (UTF-32 Big-Endian) 12001
		/// </summary>
		/// <param name="rawBytesFromFile">byte[] to analyze</param>
		/// <param name="throwOnErrors">Whether to throw a DecoderFallbackException on Errors</param>
		/// <returns>Encoding (using the fault tolerant c'tor)</returns>
		static Encoding TryGetEncodingBySignatureDetection(byte[] rawBytesFromFile, bool throwOnErrors)
		{
			Encoding encoding = null;
			if (rawBytesFromFile != null && rawBytesFromFile.Length > 0)
			{
				if (rawBytesFromFile[0] == 0xFF)
				{
					if (rawBytesFromFile.Length > 1)
					{
						if (rawBytesFromFile[1] == 0xFE)
						{
							if (rawBytesFromFile.Length > 3 &&
								rawBytesFromFile[2] == 0x00 &&
								rawBytesFromFile[3] == 0x00) // FF FE 00 00 ->  Unicode (UTF-32 LE) 12000
								encoding = Encoding.GetEncoding("utf-32"); // new UTF32Encoding(false, true, throwOnErrors);
							else
								encoding = new UnicodeEncoding(false, true, throwOnErrors);
							// FF FE -> Unicode (UTF-16 LE)1200
						}
					}
				}
				else if (rawBytesFromFile[0] == 0xFE) // FE FF -> Unicode (Big-Endian) 1201
				{
					if (rawBytesFromFile.Length > 1 && rawBytesFromFile[1] == 0xFF)
					{
						encoding = new UnicodeEncoding(true, true, throwOnErrors);
					}
				}
				else if (rawBytesFromFile[0] == 0xEF) // EF BB BF -> Unicode (UTF-8) 65001
				{
					if (rawBytesFromFile.Length > 2 && rawBytesFromFile[1] == 0xBB && rawBytesFromFile[2] == 0xBF)
					{
						encoding = new UTF8Encoding(true, throwOnErrors);
					}
				}
				else if (rawBytesFromFile[0] == 0x00)
				{
					if (rawBytesFromFile.Length > 3 &&
						rawBytesFromFile[1] == 0x00 &&
						rawBytesFromFile[2] == 0xFE &&
						rawBytesFromFile[3] == 0xFF) // 00 00 FE FF -> Unicode (UTF-32 Big-Endian) 12001
						encoding = Encoding.GetEncoding("utf-32BE"); //new UTF32Encoding(true, true, throwOnErrors);
				}
			}
			return encoding;
		}
	}
}