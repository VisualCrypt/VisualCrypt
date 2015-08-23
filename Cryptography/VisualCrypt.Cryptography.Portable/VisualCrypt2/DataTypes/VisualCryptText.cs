using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class VisualCryptText
	{
		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public string Text
		{
			get { return _text; }
		}

		readonly string _text;

		public VisualCryptText(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			_text = text;
		}
	}
}