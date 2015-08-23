using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class RoundsExponent
	{
		public byte Value
		{
			get { return _value; }
		}
		readonly byte _value;

		public RoundsExponent(byte value)
		{
			if (value < 4 || value > 31)
				throw new ArgumentOutOfRangeException("value", value, "The RoundsExponent must be between 4 and 31 (inclusive)");

			_value = value;
		}
	}
}