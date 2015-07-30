using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class RandomKey32 : SecureBytes
	{
		public RandomKey32(byte[] dataBytes) : base(dataBytes)
		{
			// perform datatype-specific validation here
			if (dataBytes.Length != 32)
				throw new ArgumentOutOfRangeException("dataBytes", "The length must be 32 bytes.");
		}
	}
}