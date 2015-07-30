using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class MessageCipher : SecureBytes
	{
		public MessageCipher(byte[] dataBytes)
			: base(dataBytes)
		{
			// perform datatype-specific validation here
		}
	}
}