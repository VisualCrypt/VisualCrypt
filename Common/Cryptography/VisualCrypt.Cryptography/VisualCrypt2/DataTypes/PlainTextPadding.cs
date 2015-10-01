using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class PlaintextPadding
	{
		public byte Value
		{
			get { return _value; }
		}
		readonly byte _value;

		public PlaintextPadding(int plaintextPadding)
		{
			if (plaintextPadding < 0 || plaintextPadding > 15)
				throw new ArgumentException("The padding amount must be >= 0 and < 16", "plaintextPadding");

			_value = (byte) plaintextPadding;
		}
	}
}