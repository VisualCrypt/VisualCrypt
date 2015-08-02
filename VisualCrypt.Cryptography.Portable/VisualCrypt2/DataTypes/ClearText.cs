using System;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
	public sealed class Cleartext
	{
		const int MaxClearTextLength = 1024 * 1024 * 10;

		// we could techniocally encrypt an empty message, but it would be obvious for the attacker that the message is empty, 
		// because the lenght of the cipher would be 0 blocks. Therefore encrpytion of empty messages is not allowed.
		const int MinClearTextLength = 1;

		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public string Text
		{
			get { return _text; }
		}
		readonly string _text;

		public Cleartext(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			if (text.Length > MaxClearTextLength)
				throw new ArgumentOutOfRangeException("text", string.Format("Text too long. Maximum is {0} characters.", MaxClearTextLength));

			if (text.Length < MinClearTextLength)
				throw new NotSupportedException("Empty messages are not allowed. An attacker would know the message is empty because empty messages have a characteristic size.");

			_text = text;
		}



	}
}