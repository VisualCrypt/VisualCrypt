namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class MessageCipher : SecureBytes
	{
		public MessageCipher(byte[] data)
			: base(data)
		{
			// perform datatype-specific validation here
		}
	}
}