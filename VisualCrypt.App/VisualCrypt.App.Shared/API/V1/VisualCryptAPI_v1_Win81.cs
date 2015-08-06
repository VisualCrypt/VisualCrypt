//using System.Runtime.InteropServices.WindowsRuntime;
//using System.Text;
//using Windows.Security.Cryptography;
//using Windows.Security.Cryptography.Core;
//using Windows.Storage.Streams;
//using System;
//using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
//using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;


//namespace VisualCrypt.App
//{
//	public class VisualCryptCoreAPI_RT_V1 : IVisualCrypt2API
//	{


//		string Encrypt_String_To_VisualCryptTextString(string clearTextString, byte[] utf16PasswordBytes)
//		{

//			IBuffer pwBuffer = CryptographicBuffer.CreateFromByteArray(utf16PasswordBytes);
//			IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary("salt", BinaryStringEncoding.Utf16LE);
//			IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(clearTextString, BinaryStringEncoding.Utf16LE);

//			// Derive key material for password size 32 bytes for AES256 algorithm
//			KeyDerivationAlgorithmProvider keyDerivationProvider = Windows.Security.Cryptography.Core.KeyDerivationAlgorithmProvider.OpenAlgorithm("PBKDF2_SHA1");
//			// using salt and 1000 iterations
//			KeyDerivationParameters pbkdf2Parms = KeyDerivationParameters.BuildForPbkdf2(saltBuffer, 1000);

//			// create a key based on original key and derivation parmaters
//			CryptographicKey keyOriginal = keyDerivationProvider.CreateKey(pwBuffer);
//			IBuffer keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal, pbkdf2Parms, 32);
//			CryptographicKey derivedPwKey = keyDerivationProvider.CreateKey(pwBuffer);

//			// derive buffer to be used for encryption salt from derived password key 
//			IBuffer saltMaterial = CryptographicEngine.DeriveKeyMaterial(derivedPwKey, pbkdf2Parms, 16);

//			// display the buffers – because KeyDerivationProvider always gets cleared after each use, they are very similar unforunately
//			string keyMaterialString = CryptographicBuffer.EncodeToBase64String(keyMaterial);
//			string saltMaterialString = CryptographicBuffer.EncodeToBase64String(saltMaterial);

//			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");

//			// create symmetric key from derived password key
//			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyMaterial);

//			// encrypt data buffer using symmetric key and derived salt material
//			IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, plainBuffer, saltMaterial);
//			byte[] cipherBytes;
//			CryptographicBuffer.CopyToByteArray(resultBuffer, out cipherBytes);

//			var cipher = new CipherV2 { MessageCipher = new MessageCipher(cipherBytes), IV16 = new IV16(saltMaterial.ToArray()) };

//			return _visualCryptFormatter.CreateVisualCryptFormat(cipher);

//			/*
//			var aesCryptographer = new AesCryptographer();
//			var textBytes = aesCryptographer.GetUTF8BytesFromUnicodeString(clearTextString);

//			var hashedPasswordBytes = aesCryptographer.CreateSha256Hash(utf16PasswordBytes);

//			byte[] iv;
//			var cipherBytes = aesCryptographer.EncryptBytes(textBytes, hashedPasswordBytes, out iv);

//			return VisualCryptFormatter.CreateVisualCryptFormat(cipherBytes, iv);
//			 */

//		}



//		public byte[] GetUTF16LEPasswordBytes(object password)
//		{
//			return CryptographicBuffer.ConvertStringToBinary((string)password, BinaryStringEncoding.Utf16LE).ToArray();
//		}

//		public byte[] GetUTF8ClearTextBytes(string utf16ClearTextString)
//		{
//			return CryptographicBuffer.ConvertStringToBinary(utf16ClearTextString, BinaryStringEncoding.Utf8).ToArray();
//		}

//		public byte[] GetKeyBytesFromUTF16LEPasswordBytes(byte[] utf16LEPasswordBytes)
//		{
//			byte[] keyBytes;
//			using (var sha256 = new Portable.Lib.SHA256ManagedMono())
//			{
//				keyBytes = sha256.ComputeHash(utf16LEPasswordBytes);
//			}
//			return keyBytes;
//		}

//		public Cipher GetCipherBytesAndIV(byte[] utf8ClearTextBytes, byte[] keyBytes)
//		{
//			var cipher = new Cipher();

//			IBuffer ivBytesBuffer = CryptographicBuffer.GenerateRandom(16);

//			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");

//			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(keyBytes);


//			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

//			IBuffer utf8ClearTextBytesBuffer = CryptographicBuffer.CreateFromByteArray(utf8ClearTextBytes);
//			// encrypt data buffer using symmetric key and derived salt material
//			IBuffer resultBuffer = CryptographicEngine.Encrypt(symmKey, utf8ClearTextBytesBuffer, ivBytesBuffer);

//			byte[] cipherBytes;
//			CryptographicBuffer.CopyToByteArray(resultBuffer, out cipherBytes);

//			cipher.IVBytes = ivBytesBuffer.ToArray();
//			cipher.CipherBytes = cipherBytes;
//			return cipher;
//		}

//		public string CreateVisualCryptText(Cipher cipher)
//		{
//			return _visualCryptFormatter.CreateVisualCryptFormat(cipher);
//		}

//		public Cipher DissectVisualCryptText(string visualCryptText)
//		{
//			return _visualCryptFormatter.DissectVisualCrypt(visualCryptText);
//		}

//		public byte[] DecryptToUTF8ClearTextBytes(Cipher cipher, byte[] keyBytes)
//		{
//			IBuffer cipherBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipher.CipherBytes);
//			IBuffer ivBytesBuffer = CryptographicBuffer.CreateFromByteArray(cipher.IVBytes);
//			IBuffer keyBytesBuffer = CryptographicBuffer.CreateFromByteArray(keyBytes);

//			SymmetricKeyAlgorithmProvider symProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");

//			CryptographicKey symmKey = symProvider.CreateSymmetricKey(keyBytesBuffer);

//			return CryptographicEngine.Decrypt(symmKey, cipherBytesBuffer, ivBytesBuffer).ToArray();
//		}

//		public string GetStringFromUTF8Bytes(byte[] utf8ClearTextBytes)
//		{
//			return Encoding.UTF8.GetString(utf8ClearTextBytes, 0, utf8ClearTextBytes.Length);
//		}

//		public byte[] GetZippedUtf8ClearBytes(string utf16ClearTextString)
//		{
//			throw new NotImplementedException();
//		}

//		public byte[] CreateSHA256PasswordBytesFromUTF16LEPasswordBytes(byte[] utf16LEPasswordBytes)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
