using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.Net.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Desktop.Tests
{
	[TestClass]
	public class EncryptionTests_V2
	{
		readonly IVisualCryptAPIV2 _visualCryptAPI;
		readonly ICoreAPIV2 _visualCryptCoreAPI;
		readonly List<string> _messages = new List<string> {""};

		public EncryptionTests_V2()
		{
			_visualCryptCoreAPI = new CoreAPIV2_Net4();
			_visualCryptAPI = new VisualCryptAPIV2(_visualCryptCoreAPI);

			var message = _messages[0];

			for (var i = 0; i <= 35; i++)
			{
				message += "1";
				_messages.Add(message);
			}
		}


		[TestMethod]
		public void CanEncryptAndDecryptWithRightPassword()
		{
			foreach (var m in _messages)
			{
				var hashPasswordResponse = _visualCryptAPI.CreateSHA256PW32("Password" + m);

				if (!hashPasswordResponse.Success)
					Assert.Fail("Password hashing failed");

				// do the encryption
				string visualCrypt;
				var encryptResponse = _visualCryptAPI.Encrypt(new ClearText(m), hashPasswordResponse.Result, new BWF(5));
				if (encryptResponse.Success)
				{
					var encodeResponse = _visualCryptAPI.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.Success)
					{
						visualCrypt = encodeResponse.Result.Value;
					}
					else
						throw new Exception(encodeResponse.Error);
				}
				else
					throw new Exception(encryptResponse.Error);

				// do the decryption
				string decryptedMessage;

				var decodeResponse = _visualCryptAPI.TryDecodeVisualCryptText(visualCrypt);

				if (decodeResponse.Success)
				{
					var decryptResponse = _visualCryptAPI.Decrypt(decodeResponse.Result,
						hashPasswordResponse.Result);
					if (decryptResponse.Success)
						decryptedMessage = decryptResponse.Result.Value;
					else
						throw new Exception(decryptResponse.Error);
				}
				else
					throw new Exception(decodeResponse.Error);

				Assert.IsTrue(decryptedMessage.Equals(m),
					"The decrypted message ('{0}') does not equal the original message".FormatInvariant(decryptedMessage));
				Console.WriteLine("Testing with m/pw length: " + m.Length + " Message: " + m);
			}
		}

		[TestMethod]
		public void CannotEncryptAndDecryptWithWrongPassword()
		{
			foreach (var m in _messages)
			{
				var password = m;

				if (password.Equals(string.Empty))
					continue;

				var hashPasswordResponse = _visualCryptAPI.CreateSHA256PW32("Password" + m);

				// do the encryption
				string visualCrypt;
				var encryptResponse = _visualCryptAPI.Encrypt(new ClearText(m), hashPasswordResponse.Result, new BWF(5));
				if (encryptResponse.Success)
				{
					var encodeResponse = _visualCryptAPI.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.Success)
					{
						visualCrypt = encodeResponse.Result.Value;
					}
					else
						throw new Exception(encodeResponse.Error);
				}
				else
					throw new Exception(encryptResponse.Error);

				// set a wrong password
				hashPasswordResponse = _visualCryptAPI.CreateSHA256PW32("Wrong Password");

				// do the decryption
				var decodeResponse = _visualCryptAPI.TryDecodeVisualCryptText(visualCrypt);

				if (decodeResponse.Success)
				{
					var decryptResponse = _visualCryptAPI.Decrypt(decodeResponse.Result,
						hashPasswordResponse.Result);
					if (decryptResponse.Success)
						Assert.Fail("decryptResponse.Success MUST NOT be true with wrong password!");
					Console.WriteLine("Cannot decrypt with wrong password - OK!");
				}
				else
					throw new Exception(decodeResponse.Error);
			}
		}

		[TestMethod]
		public void PaddingIsAppliedCorrectlyBeforeEncryption()
		{
			Assert.Fail("Test needed There are conditions where an Exception from Padding is thrown, encrypting certain tests.");
		}
	}
}