namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations
{
	public interface ICoreAPI2
	{
		byte[] GenerateRandomBytes(int length);

		byte[] ComputeSHA512(byte[] data);

		byte[] ComputeSHA256(byte[] data);

        byte[] ComputeAESRound(AESDir aesDir, byte[] currentIV, byte[] inputData, byte[] keyBytes);
    }
}