using System;
using System.Linq;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class BWF
	{
		public readonly byte Value;

		public BWF(byte value)
		{
			if (value < 4 || value > 31)
				throw new ArgumentOutOfRangeException("value", value, "The BCrypt work factor must be between 4 and 31 (inclusive)");

			Value = value;
		}
	}
}