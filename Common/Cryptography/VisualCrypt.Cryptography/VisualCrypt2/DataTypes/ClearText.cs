using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class Cleartext
	{
		const int MaxClearTextLength = 1024 * 1024 * 10;

		// Empty messages are not secret, because the lenght of the cipher is always 0 blocks. Allow lenght of 0?
		const int MinClearTextLength = 0;

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
				throw new NotSupportedException(string.Format("Text too short. Minimum is {0} characters.", MinClearTextLength));

			_text = text;
		}



	}
}