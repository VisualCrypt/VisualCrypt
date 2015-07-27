namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class CipherV2
	{
		public const byte Version = 2;

		public BWF BWF; // BCrypt Work Factor

		public byte Padding;

		public IV16 IV16;

		public byte[] RandomKeyCipher;

		public byte[] MessageCipher;

		public byte[] MACCipher;

		
	}
}