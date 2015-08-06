using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using VisualCrypt.Cryptography.Net.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.App.API.V2
{
	public class Core2API : ICoreAPI2
	{
		public Compressed Compress(Cleartext cleartext)
		{

			Guard.NotNull(cleartext);

			byte[] compressed = Deflate.Compress(cleartext.Text, Encoding.UTF8);

			return new Compressed(compressed);
		}

		public byte[] GenerateRandomBytes(int length)
		{
			IBuffer random = CryptographicBuffer.GenerateRandom((uint)length);
			return random.ToArray();
		}

		public PaddedData ApplyRandomPadding(Compressed compressed)
		{
			Guard.NotNull(compressed);

			int requiredPadding;
			if (compressed.GetBytes().Length % 16 == 0)
				requiredPadding = 0;
			else
				requiredPadding = 16 - compressed.GetBytes().Length % 16;

			var paddingBytes = GenerateRandomBytes(requiredPadding);

			var paddedDataBytes = new byte[compressed.GetBytes().Length + requiredPadding];
			System.Buffer.BlockCopy(compressed.GetBytes(), 0, paddedDataBytes, 0, compressed.GetBytes().Length);
			System.Buffer.BlockCopy(paddingBytes, 0, paddedDataBytes, compressed.GetBytes().Length, paddingBytes.Length);

			return new PaddedData(paddedDataBytes, new PlaintextPadding(requiredPadding));
		}

		public void AESEncryptRandomKeyWithPasswordDerivedKey(PasswordDerivedKey32 passwordDerivedKey, RandomKey32 randomKey, CipherV2 cipherV2, LongRunningOperationContext context)
		{


			IBuffer ivBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.IV16.GetBytes());

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");  // Padding: None

			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(passwordDerivedKey.GetBytes());


			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

			IBuffer utf8ClearTextBytesBuffer = CryptographicBuffer.CreateFromByteArray(randomKey.GetBytes());
			// encrypt data buffer using symmetric key and derived salt material
			IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, utf8ClearTextBytesBuffer, ivBytesBuffer);

			byte[] cipherBytes;
			CryptographicBuffer.CopyToByteArray(resultBuffer, out cipherBytes);
			cipherV2.RandomKeyCipher32 = new RandomKeyCipher32(cipherBytes);

		}

		public void AESEncryptMessageWithRandomKey(PaddedData paddedData, RandomKey32 randomKey, CipherV2 cipherV2, LongRunningOperationContext context)
		{
			IBuffer ivBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.IV16.GetBytes());

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");  // Padding: None

			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(randomKey.GetBytes());


			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

			IBuffer utf8ClearTextBytesBuffer = CryptographicBuffer.CreateFromByteArray(paddedData.GetBytes());
			// encrypt data buffer using symmetric key and derived salt material
			IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, utf8ClearTextBytesBuffer, ivBytesBuffer);

			byte[] cipherBytes;
			CryptographicBuffer.CopyToByteArray(resultBuffer, out cipherBytes);
			cipherV2.MessageCipher = new MessageCipher(cipherBytes);
			cipherV2.Padding = new PlaintextPadding(paddedData.PlaintextPadding.ByteValue);
		}

		public void AESEncryptMACWithRandomKey(CipherV2 cipherV2, MAC16 mac, RandomKey32 randomKey, LongRunningOperationContext context)
		{
			IBuffer ivBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.IV16.GetBytes());

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");  // Padding: None

			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(randomKey.GetBytes());


			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

			IBuffer utf8ClearTextBytesBuffer = CryptographicBuffer.CreateFromByteArray(mac.GetBytes());
			// encrypt data buffer using symmetric key and derived salt material
			IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, utf8ClearTextBytesBuffer, ivBytesBuffer);

			byte[] cipherBytes;
			CryptographicBuffer.CopyToByteArray(resultBuffer, out cipherBytes);
			cipherV2.MACCipher16 = new MACCipher16(cipherBytes);
		}

		public MAC16 AESDecryptMAC(CipherV2 cipherV2, RandomKey32 randomKey, LongRunningOperationContext context)
		{

			IBuffer cipherBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.MACCipher16.GetBytes());
			IBuffer ivBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.IV16.GetBytes());
			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(randomKey.GetBytes());

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");

			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

			var result = CryptographicEngine.Decrypt(symmKey, cipherBytesBuffer, ivBytesBuffer).ToArray();
			return new MAC16(result);
		}

		public RandomKey32 AESDecryptRandomKeyWithPasswordDerivedKey(CipherV2 cipherV2, PasswordDerivedKey32 passwordDerivedKey, LongRunningOperationContext context)
		{
			IBuffer cipherBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.RandomKeyCipher32.GetBytes());
			IBuffer ivBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.IV16.GetBytes());
			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(passwordDerivedKey.GetBytes());

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");

			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

			var result = CryptographicEngine.Decrypt(symmKey, cipherBytesBuffer, ivBytesBuffer).ToArray();
			return new RandomKey32(result);
		}

		public PaddedData AESDecryptMessage(CipherV2 cipherV2, IV16 iv16, RandomKey32 randomKey, LongRunningOperationContext context)
		{
			IBuffer cipherBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.MessageCipher.GetBytes());
			IBuffer ivBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipherV2.IV16.GetBytes());
			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(randomKey.GetBytes());

			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC");

			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

			var result = CryptographicEngine.Decrypt(symmKey, cipherBytesBuffer, ivBytesBuffer).ToArray();
			return new PaddedData(result, cipherV2.Padding);
		}

		public Compressed RemovePadding(PaddedData paddedData)
		{
			Guard.NotNull(paddedData);

			var paddingRemoved = new byte[paddedData.GetBytes().Length - paddedData.PlaintextPadding.ByteValue];

			System.Buffer.BlockCopy(paddedData.GetBytes(), 0, paddingRemoved, 0, paddingRemoved.Length);

			return new Compressed(paddingRemoved);
		}

		public Cleartext Decompress(Compressed compressed)
		{
			Guard.NotNull(compressed);

			var clearText = Deflate.Decompress(compressed.GetBytes(), Encoding.UTF8);
			return new Cleartext(clearText);
		}

		public string GenerateRandomPassword()
		{
			throw new NotImplementedException();
		}

		public byte[] ComputeSHA512(byte[] data)
		{


			// put the string in a buffer, UTF-8 encoded...
			IBuffer input = CryptographicBuffer.CreateFromByteArray(data);

			// hash it...
			var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA512");
			IBuffer hashed = hasher.HashData(input);
			return hashed.ToArray();
			// format it...
			//this.textBase64.Text = CryptographicBuffer.EncodeToBase64String(hashed);
			//this.textHex.Text = CryptographicBuffer.EncodeToHexString(hashed);

		}

		public byte[] ComputeSHA256(byte[] data)
		{
			// put the string in a buffer, UTF-8 encoded...
			IBuffer input = CryptographicBuffer.CreateFromByteArray(data);

			// hash it...
			var hasher = HashAlgorithmProvider.OpenAlgorithm("SHA256");
			IBuffer hashed = hasher.HashData(input);
			return hashed.ToArray();
		}
	}
}
