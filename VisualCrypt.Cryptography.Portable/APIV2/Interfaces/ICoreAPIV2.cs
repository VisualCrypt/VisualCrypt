using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;

namespace VisualCrypt.Cryptography.Portable.APIV2.Interfaces
{
	public interface ICoreAPIV2
	{
		Compressed Compress(ClearText clearText);

		IV16 GenerateIV(int length);

		PaddedData ApplyRandomPadding(Compressed compressed);

		CipherV2 AESEncryptMessage(PaddedData paddedData, AESKey32 aesKey32, IV16 outerIV16);

		void AESEncryptMessageDigest(CipherV2 cipherv2, MD16 md16, AESKey32 aesKey32);

		MD16 AESDecryptMessageDigest(MD16E mD16E, IV16 iV16, AESKey32 aesKey32);

		PaddedData AESDecryptMessage(CipherV2 cipherV2, IV16 iV16, AESKey32 aesKey32);

		Compressed RemovePadding(PaddedData paddedData);

		ClearText Decompress(Compressed compressed);

		string GenerateRandomPassword();
	}
}