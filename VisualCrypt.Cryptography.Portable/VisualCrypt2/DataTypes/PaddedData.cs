using System;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
	public sealed class PaddedData : SecureBytes
	{
		public PlaintextPadding PlaintextPadding
		{
			get { return _plaintextPadding; }
		}

		readonly PlaintextPadding _plaintextPadding;

		public PaddedData(byte[] data, PlaintextPadding plaintextPadding)
			: base(data)
		{
			// perform datatype-specific validation here
			if (plaintextPadding == null)
				throw new ArgumentNullException("plaintextPadding");

			_plaintextPadding = plaintextPadding;
		}
	}
}