using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class RandomKeyCipher32 : SecureBytes
	{
		public RandomKeyCipher32(byte[] data)
			: base(data)
		{
			// perform datatype-specific validation here
			if (data.Length != 32)
				throw new ArgumentOutOfRangeException("data", "The length must be 32 bytes.");
		}
	}
}