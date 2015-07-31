using System;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
	public sealed class SHA512PW64 : SecureBytes
	{
		public SHA512PW64(byte[] data) : base(data)
		{
			// perform datatype-specific validation here
			if (data.Length != 64)
				throw new ArgumentOutOfRangeException("data", "The length must be 64 bytes.");
		}
	}
}