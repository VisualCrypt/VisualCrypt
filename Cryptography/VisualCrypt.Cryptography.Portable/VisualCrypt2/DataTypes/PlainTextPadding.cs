using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class PlaintextPadding
	{
		public byte ByteValue
		{
			get { return _byteValue; }
		}
		readonly byte _byteValue;

		public PlaintextPadding(int plaintextPadding)
		{
			if (plaintextPadding < 0 || plaintextPadding > 15)
				throw new ArgumentException("The padding amount must be >= 0 and < 16", "plaintextPadding");

			_byteValue = (byte) plaintextPadding;
		}
	}
}