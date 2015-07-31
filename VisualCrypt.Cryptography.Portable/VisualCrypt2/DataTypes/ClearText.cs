using System;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
	public sealed class ClearText
	{
		const int MaxClearTextLength = 1024 * 1024 * 10;

		const int MinClearTextLength = 0;

		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public string Text
		{
			get { return _text; }
		}
		readonly string _text;

		public ClearText(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			if (text.Length > MaxClearTextLength)
				throw new ArgumentOutOfRangeException("text", "Text too long.");

			if (text.Length < MinClearTextLength)
				throw new ArgumentOutOfRangeException("text",
					"Text with a length shorter than {0} is not supported.".FormatInvariant(MinClearTextLength));

			_text = text;
		}

		

	}
}