namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class CipherV2
	{
		public const byte Version = 2;

		public RoundsExponent RoundsExponent { get; set; }

		public PlaintextPadding PlaintextPadding { get; set; }

		public IV16 IV16 { get; set; }

        public MACCipher16 MACCipher16 { get; set; }

        public RandomKeyCipher32 RandomKeyCipher32 { get; set; }

		public MessageCipher MessageCipher { get; set; }

		
	}
}