using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class MAC16 : SecureBytes
	{
		public MAC16(byte[] dataBytes) :base(dataBytes)
		{
			// perform datatype-specific validation here
			if (dataBytes.Length != 16)
				throw new ArgumentOutOfRangeException("dataBytes", "The length must be 16 bytes.");
		}
	}
}