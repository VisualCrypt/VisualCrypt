namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
	public sealed class Compressed : SecureBytes
	{
		public Compressed(byte[] data) : base(data)
		{
			// perform datatype-specific validation here
		}
	}
}