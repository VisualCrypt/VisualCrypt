using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class VisualCryptText
	{
		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public readonly string StringValue;

		public VisualCryptText(string stringValue)
		{
			if (stringValue == null)
				throw new ArgumentNullException("stringValue");

			StringValue = stringValue;
		}
	}
}