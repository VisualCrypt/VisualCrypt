namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class Compressed : SecureBytes
	{
		public Compressed(byte[] dataBytes) : base(dataBytes)
		{
			// perform datatype-specific validation here
		}
	}
}