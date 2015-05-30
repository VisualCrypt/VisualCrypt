using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class ClearText
	{
		public const int MaxClearTextLength = 1024*1024*10;

		public const int MinClearTextLength = 0;

		/// <summary>
		/// The non-null DataBytes.
		/// </summary>
		public readonly string Value;

		public ClearText(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (value.Length > MaxClearTextLength)
				throw new ArgumentOutOfRangeException("value", "Text too long.");

			if (value.Length < MinClearTextLength)
				throw new ArgumentOutOfRangeException("value",
					string.Format("Text with a length shorter than {0} is not supported.", MinClearTextLength));

			Value = value;
		}
	}
}