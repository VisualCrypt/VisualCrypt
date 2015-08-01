using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations
{
	public interface ICoreAPI2
	{
		Compressed Compress(Cleartext cleartext);

		byte[] GenerateRandomBytes(int length);

		PaddedData ApplyRandomPadding(Compressed compressed);


		void AESEncryptRandomKeyWithPasswordDerivedKey(PasswordDerivedKey32 passwordDerivedKey, RandomKey32 randomKey, CipherV2 cipherV2, LongRunningOperationContext context);

		void AESEncryptMessageWithRandomKey(PaddedData paddedData, RandomKey32 randomKey, CipherV2 cipherV2, LongRunningOperationContext context);


		void AESEncryptMACWithRandomKey(CipherV2 cipherV2, MAC16 mac, RandomKey32 randomKey, LongRunningOperationContext context);

		MAC16 AESDecryptMAC(CipherV2 cipherV2, RandomKey32 randomKey, LongRunningOperationContext context);


		RandomKey32 AESDecryptRandomKeyWithPasswordDerivedKey(CipherV2 cipherV2, PasswordDerivedKey32 passwordDerivedKey, LongRunningOperationContext context);

		PaddedData AESDecryptMessage(CipherV2 cipherV2, IV16 iv16, RandomKey32 randomKey, LongRunningOperationContext context);


		Compressed RemovePadding(PaddedData paddedData);

		Cleartext Decompress(Compressed compressed);

		string GenerateRandomPassword();


		byte[] ComputeSHA512(byte[] data);

		byte[] ComputeSHA256(byte[] data);
	}
}