using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class BCrypt24 : SecureBytes
	{
		public BCrypt24(byte[] dataBytes) : base(dataBytes)
		{
			// perform datatype-specific validation here
			if (dataBytes.Length != 24)
				throw new ArgumentOutOfRangeException("dataBytes", "The length must be 24 bytes.");
		}
	}
}