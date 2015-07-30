using System;
using System.Text;
using System.Threading;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;

namespace VisualCrypt.Cryptography.Portable.APIV2.Interfaces
{
	public interface IVisualCryptAPIV2
	{

		Response<SHA512PW64> CreateSHA512PW64(string unsanitizedUTF16LEPassword);

		Response<CipherV2> Encrypt(ClearText clearText, SHA512PW64 sha512PW64, RoundsExp roundsExp, IProgress<int> progress, CancellationToken cToken);

		Response<VisualCryptText> EncodeToVisualCryptText(CipherV2 cipherV2);

		Response<CipherV2> TryDecodeVisualCryptText(string visualCryptText);


		Response<ClearText> Decrypt(CipherV2 cipherV2, SHA512PW64 sha512PW64, IProgress<int> progress, CancellationToken cToken);


		Response<string, Encoding> GetStringFromFileBytes(byte[] rawBytesFromFile, Encoding platformDefaultEncoding = null);

		Response<string> GenerateRandomPassword();

		Response<SanitizedPassword> SanitizePassword(string unsanitizedPassword);
	}
}