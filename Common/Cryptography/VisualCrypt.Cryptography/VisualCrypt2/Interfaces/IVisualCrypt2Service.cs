using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.VisualCrypt2.Interfaces
{
    public interface IVisualCrypt2Service
    {
        IPlatform Platform { set; }

        Response<QualifiedRandom> TestRandomNumberGeneration(int sampleSize, int randomLenght);

        Response<string> SuggestRandomPassword();

        Response<NormalizedPassword> NormalizePassword(string rawPassword);

        Response<SHA512PW64> HashPassword(NormalizedPassword normalizedPassword);

        Response<CipherV2> Encrypt(Cleartext cleartext, SHA512PW64 sha512PW64, RoundsExponent roundsExponent, LongRunningOperationContext context);

        Response<Cleartext> Decrypt(CipherV2 cipherV2, SHA512PW64 sha512PW64, LongRunningOperationContext context);

        Response<VisualCryptText> EncodeVisualCrypt(CipherV2 cipherV2);

        Response<CipherV2> DecodeVisualCrypt(string visualCryptText, LongRunningOperationContext context);
    }
}