using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class ClearText
	{
		const int MaxClearTextLength = 1024*1024*10;

		const int MinClearTextLength = 0;

		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public readonly string StringValue;

		public ClearText(string stringValue)
		{
			if (stringValue == null)
				throw new ArgumentNullException("stringValue");

			if (stringValue.Length > MaxClearTextLength)
				throw new ArgumentOutOfRangeException("stringValue", "Text too long.");

			if (stringValue.Length < MinClearTextLength)
				throw new ArgumentOutOfRangeException("stringValue",
					string.Format("Text with a length shorter than {0} is not supported.", MinClearTextLength));

			StringValue = stringValue;
		}
	}
}