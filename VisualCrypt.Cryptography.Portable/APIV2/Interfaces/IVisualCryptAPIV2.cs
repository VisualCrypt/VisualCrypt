using System;
using System.Text;
using System.Threading;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;

namespace VisualCrypt.Cryptography.Portable.APIV2.Interfaces
{
	public interface IVisualCryptAPIV2
	{
		Response<SHA256PW32> CreateSHA256PW32(byte[] utf16LEPassword);

		Response<SHA256PW32> CreateSHA256PW32(string utf16LEPassword);

		Response<CipherV2> Encrypt(ClearText clearText, SHA256PW32 sha256PW32, BWF bwf, IProgress<int> progress, CancellationToken cToken);

		Response<VisualCryptText> EncodeToVisualCryptText(CipherV2 cipherV2);

		Response<CipherV2> TryDecodeVisualCryptText(string visualCryptText);

		Response<ClearText> Decrypt(CipherV2 cipherV2, SHA256PW32 sha256PW32, IProgress<int> progress, CancellationToken cToken);

		Response<string, Encoding> GetStringFromFileBytes(byte[] rawBytesFromFile, Encoding platformDefaultEncoding = null);
	}
}