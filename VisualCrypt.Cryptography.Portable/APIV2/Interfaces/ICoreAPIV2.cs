using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;

namespace VisualCrypt.Cryptography.Portable.APIV2.Interfaces
{
	public interface ICoreAPIV2
	{
		Compressed Compress(ClearText clearText);

		byte[] GenerateRandomBytes(int length);

		PaddedData ApplyRandomPadding(Compressed compressed);

		void AESEncryptRandomKeyWithPasswordDerivedKey(PasswordDerivedKey32 passwordDerivedKey, RandomKey32 randomKey, CipherV2 cipherV2);

		void AESEncryptMessageWithRandomKey(PaddedData paddedData, RandomKey32 randomKey, CipherV2 cipherV2);
		

		void AESEncryptMACWithRandomKey(CipherV2 cipherv2, MAC16 mac, RandomKey32 randomKey);

		MAC16 AESDecryptMAC(CipherV2 cipherV2, RandomKey32 randomKey);

		RandomKey32 AESDecryptRandomKeyWithPasswordDerivedKey(CipherV2 cipherV2, PasswordDerivedKey32 passwordDerivedKey);

		PaddedData AESDecryptMessage(CipherV2 cipherV2, IV16 iV16, RandomKey32 randomKey);

	

		Compressed RemovePadding(PaddedData paddedData);

		ClearText Decompress(Compressed compressed);

		string GenerateRandomPassword();


		byte[] ComputeSHA512(byte[] bytesToHash);

		byte[] ComputeSHA256(byte[] bytesToHash);
	}
}