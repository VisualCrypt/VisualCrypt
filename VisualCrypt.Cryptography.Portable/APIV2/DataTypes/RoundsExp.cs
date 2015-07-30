using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class RoundsExp
	{
		public readonly byte ByteValue;

		public RoundsExp(byte byteValue)
		{
			if (byteValue < 4 || byteValue > 31)
				throw new ArgumentOutOfRangeException("byteValue", byteValue, "The RoundsExp must be between 4 and 31 (inclusive)");

			ByteValue = byteValue;
		}
	}
}