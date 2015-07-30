using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class SHA512PW64 : SecureBytes
	{
		public SHA512PW64(byte[] dataBytes) : base(dataBytes)
		{
			// perform datatype-specific validation here
			if (dataBytes.Length != 64)
				throw new ArgumentOutOfRangeException("dataBytes", "The length must be 64 bytes.");
		}
	}
}