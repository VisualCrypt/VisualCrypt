
namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
    public sealed class CipherV2
    {
        public const byte Version = 2;

        public byte Padding;

        public IV16 IV16;

        public byte[] CipherBytes;

        public MD16E MD16E;
    }
}
