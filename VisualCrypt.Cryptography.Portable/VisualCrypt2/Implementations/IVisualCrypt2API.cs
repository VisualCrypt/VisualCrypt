using System;
using System.Text;
using System.Threading;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations
{
	public interface IVisualCrypt2API
	{
		Response<SHA512PW64> CreateSHA512PW64(string unpruned);

		Response<CipherV2> Encrypt(ClearText clearText, SHA512PW64 sha512PW64, RoundsExponent roundsExponent, IProgress<int> progress,
			CancellationToken token);

		Response<VisualCryptText> EncodeToVisualCryptText(CipherV2 cipherV2);

		Response<CipherV2> TryDecodeVisualCryptText(string visualCryptText);

		Response<ClearText> Decrypt(CipherV2 cipherV2, SHA512PW64 sha512PW64, IProgress<int> progress,
			CancellationToken token);

		Response<string, Encoding> GetStringFromFile(byte[] data, Encoding platformDefaultEncoding);

		Response<string> GenerateRandomPassword();

		Response<PrunedPassword> PrunePassword(string unprunedPassword);
	}
}