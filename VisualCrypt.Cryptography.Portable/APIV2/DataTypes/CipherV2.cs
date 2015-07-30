namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class CipherV2
	{
		public const byte Version = 2;

		public RoundsExp RoundsExp;

		public PlainTextPadding Padding;

		public IV16 IV16;

		public RandomKeyCipher32 RandomKeyCipher32;

		public MessageCipher MessageCipher;

		public MACCipher16 MACCipher16;

		
	}
}