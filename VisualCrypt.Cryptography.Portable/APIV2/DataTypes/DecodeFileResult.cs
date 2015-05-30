using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class DecodeFileResult
	{
		public readonly ClearText ClearText;

		public readonly VisualCryptText VisualCryptText;

		public readonly byte[] FileBytes;

		public readonly ContentKind ContentKind;

		public DecodeFileResult(ClearText clearText)
		{
			if (clearText == null)
				throw new ArgumentNullException("clearText");

			ClearText = clearText;
			ContentKind = ContentKind.PlainText;
		}

		public DecodeFileResult(VisualCryptText visualCryptText)
		{
			if (visualCryptText == null)
				throw new ArgumentNullException("visualCryptText");

			VisualCryptText = visualCryptText;
			ContentKind = ContentKind.EncryptedText;
		}

		public DecodeFileResult(byte[] fileBytes, ContentKind contentKind)
		{
			if (fileBytes == null)
				throw new ArgumentNullException("fileBytes");

			if (contentKind == ContentKind.PlainText || contentKind == ContentKind.EncryptedText)
				throw new ArgumentException("A non-text ContentKind is expected.", "contentKind");

			FileBytes = fileBytes;
			ContentKind = contentKind;
		}
	}
}