using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.Net.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.Portable.VisualCrypt2;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Desktop.Tests
{
	[TestClass]
	public class EncryptionTests_V2
	{
		readonly IVisualCrypt2API _visualCryptAPI;
		readonly ICoreAPI2 _visualCryptCoreAPI2;
		readonly List<string> _messages = new List<string> { "" };

		public EncryptionTests_V2()
		{
			_visualCryptCoreAPI2 = new CoreAPI2_Net4();
			_visualCryptAPI = new VisualCrypt2API(_visualCryptCoreAPI2);

			var message = _messages[0];

			for (var i = 0; i <= 300; i++)
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
				var hashPasswordResponse = _visualCryptAPI.CreateSHA512PW64("Password" + m);

				if (!hashPasswordResponse.IsSuccess)
					Assert.Fail("Password hashing failed");

				// do the encryption
				string visualCrypt;
				var encryptResponse = _visualCryptAPI.Encrypt(new Cleartext(m), hashPasswordResponse.Result, new RoundsExponent(4), CreateContext());
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCryptAPI.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						visualCrypt = encodeResponse.Result.Text;
					}
					else
						throw new Exception(encodeResponse.Error);
				}
				else
					throw new Exception(encryptResponse.Error);

				// do the decryption
				string decryptedMessage;

				var decodeResponse = _visualCryptAPI.TryDecodeVisualCryptText(visualCrypt);

				if (decodeResponse.IsSuccess)
				{
					var decryptResponse = _visualCryptAPI.Decrypt(decodeResponse.Result,
						hashPasswordResponse.Result, CreateContext());
					if (decryptResponse.IsSuccess)
						decryptedMessage = decryptResponse.Result.Text;
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

		LongRunningOperationContext CreateContext()
		{
			return new LongRunningOperationContext(new CancellationTokenSource().Token, new Progress<int>());
		}

		[TestMethod]
		public void CannotEncryptAndDecryptWithWrongPassword()
		{
			foreach (var m in _messages)
			{
				var password = m;

				if (password.Equals(string.Empty))
					continue;

				var hashPasswordResponse = _visualCryptAPI.CreateSHA512PW64("Password" + m);

				// do the encryption
				string visualCrypt;
				var encryptResponse = _visualCryptAPI.Encrypt(new Cleartext(m), hashPasswordResponse.Result, new RoundsExponent(5), CreateContext());
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCryptAPI.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						visualCrypt = encodeResponse.Result.Text;
					}
					else
						throw new Exception(encodeResponse.Error);
				}
				else
					throw new Exception(encryptResponse.Error);

				// set a wrong password
				hashPasswordResponse = _visualCryptAPI.CreateSHA512PW64("Wrong Password");

				// do the decryption
				var decodeResponse = _visualCryptAPI.TryDecodeVisualCryptText(visualCrypt);

				if (decodeResponse.IsSuccess)
				{
					var decryptResponse = _visualCryptAPI.Decrypt(decodeResponse.Result,
						hashPasswordResponse.Result, CreateContext());
					if (decryptResponse.IsSuccess)
						Assert.Fail("decryptResponse.SUCCESS MUST NOT be true with wrong password!");
					Console.WriteLine("Cannot decrypt with wrong password - OK!");
				}
				else
					throw new Exception(decodeResponse.Error);
			}
		}

		[TestMethod]
		public void PaddingIsAppliedCorrectlyBeforeEncryption()
		{
			var stringWhereTheErrorOccures = "vyxvxvxyvxvyvcxyvyx";

			var hashPasswordResponse = _visualCryptAPI.CreateSHA512PW64("Password");

			// do the encryption
			string visualCrypt;
			var encryptResponse = _visualCryptAPI.Encrypt(new Cleartext(stringWhereTheErrorOccures), hashPasswordResponse.Result, new RoundsExponent(5),  CreateContext());
			if (encryptResponse.IsSuccess)
			{
				var encodeResponse = _visualCryptAPI.EncodeToVisualCryptText(encryptResponse.Result);
				if (encodeResponse.IsSuccess)
				{
					visualCrypt = encodeResponse.Result.Text;
				}
				else
					throw new Exception(encodeResponse.Error);
			}
			else
				throw new Exception(encryptResponse.Error);


			// do the decryption
			var decodeResponse = _visualCryptAPI.TryDecodeVisualCryptText(visualCrypt);

			if (decodeResponse.IsSuccess)
			{
				var decryptResponse = _visualCryptAPI.Decrypt(decodeResponse.Result,
					hashPasswordResponse.Result, CreateContext());
				if (!decryptResponse.IsSuccess)
					Assert.Fail("Expecteed decryption to work!");
			}
			else
				throw new Exception(decodeResponse.Error);

		}

		[TestMethod]
		public void ModuloTest()
		{
			var res = 0 % 16;
			Console.WriteLine(res);
			Assert.IsTrue(res == 0);

		}
	}
}