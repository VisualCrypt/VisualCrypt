using System;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
	public sealed class PasswordDerivedKey32 : SecureBytes
	{
		public PasswordDerivedKey32(byte[] data) : base(data)
		{
			// perform datatype-specific validation here
			if (data.Length != 32)
				throw new ArgumentOutOfRangeException("data", "The length must be 32 bytes.");
		}
	}
}