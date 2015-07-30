using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class PlainTextPadding
	{
		public readonly byte ByteValue;

		public PlainTextPadding(int plainTextPadding)
		{
			if (plainTextPadding < 0 || plainTextPadding > 15)
				throw new ArgumentException("The padding amount must be >= 0 and < 16", "plainTextPadding");

			ByteValue = (byte) plainTextPadding;
		}
	}
}