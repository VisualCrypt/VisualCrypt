using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.Net.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.Cryptography.Net.Tests
{
    /// <summary>
    ///     Tests the basic functionality of VisualCrypt2API with correct
    ///     or obviously incorrect parameters.
    ///     Demonstrates the use of the Response class. Members of VisualCrypt2API must not throw
    ///     Exceptions. Instead, a success is indicated with IsSuccess == true and failure with IsSuccess
    ///     == false. In case of an internal error Response.Error carries the error message.
    /// </summary>
    [TestClass]
    public class Basic_Member_Tests
    {
        readonly IVisualCrypt2API _api;
        readonly IReadOnlyCollection<string> _strings;

        public Basic_Member_Tests()
        {
            IPlatform platform = new Platform_Net4();
            _api = new VisualCrypt2API(platform);

            _strings = CreateStringsOfVariousLenghts();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Platform_Is_Required()
        {
            IPlatform platform = null;
            new VisualCrypt2API(platform);
        }

        [TestMethod]
        public void Versioning_Enforced()
        {
            Assert.AreEqual(2, CipherV2.Version);
        }


        [TestMethod]
        public void Member_CreateSHA512PW64()
        {
            SHA512PW64 sha512PW64 = CreatePasswordHash("some password");
            Assert.IsTrue(sha512PW64.GetBytes().Length == 64);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Member_CreateSHA512PW64_Fails_With_null()
        {
            CreatePasswordHash(null);
        }

        [TestMethod]
        public void Member_CreateSHA512PW64_Accepts_Empty_Password()
        {
            SHA512PW64 sha512PW64 = CreatePasswordHash("");
            Assert.IsTrue(sha512PW64.GetBytes().Length == 64);
        }


        [TestMethod]
        public void Member_Encrypt()
        {
            var secret = new Cleartext("some secret");
            var passwordHash = CreatePasswordHash("some password");
            var rounds = new RoundsExponent(4);

            var response = _api.Encrypt(secret, passwordHash, rounds, CreateContext());

            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public void Member_Encrypt_Context_Is_Required()
        {
            var secret = new Cleartext("some secret");
            var passwordHash = CreatePasswordHash("some password");
            var rounds = new RoundsExponent(4);

            var response = _api.Encrypt(secret, passwordHash, rounds, null);

            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public void Member_Decrypt_Arbitrary_Data_Causes_Password_Error()
        {
            var cipherV2 = CreateArbitraryCipherV2();
            var passwordHash = CreatePasswordHash("some password");

            var response = _api.Decrypt(cipherV2, passwordHash, CreateContext());

            Assert.IsFalse(response.IsSuccess);
            Assert.IsTrue(response.Error == LocalizableStrings.MsgPasswordError, "Expected 'Password Error' when decrypting arbitrary data");
        }

        [TestMethod]
        public void Member_Decrypt_Can_Decrypt_With_Correct_Password()
        {
            var cipherV2 = CreateCipherV2("my password", "my message");
            var passwordHash = CreatePasswordHash("my password");

            var response = _api.Decrypt(cipherV2, passwordHash, CreateContext());

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(response.Result.Text, "my message");
        }

        [TestMethod]
        public void Member_Decrypt_Can_Decrypt_Specific_Message()
        {
            var specificMessage = "VisualCrypt/AgoDqjwFKw84kRUVeg3hUfVMiP7Yr9lAlpOT2Af+D10ly$zLQ6UrAWZyHHspGYP57e4Xtj7mYNIx4ZHhRbII7WAP8GAOsHPkt6ZJrmf4$YjjHYA = ";
            CipherV2 decoded = _api.TryDecodeVisualCryptText(specificMessage).Result;
            var passwordHash = CreatePasswordHash("the password");

            var response = _api.Decrypt(decoded, passwordHash, CreateContext());

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(response.Result.Text, "Hello World", "If this fails we've broken compatibility.");
        }


        [TestMethod]
        public void Can_Encrypt_And_Decrypt_With_Correct_Password()
        {
            foreach (var m in _strings)
            {
                var message = m + "message";
                var password = m + "password";

                var hashPasswordResponse = _api.CreateSHA512PW64(password);

                // encrypt
                string visualCrypt;
                var encryptResponse = _api.Encrypt(new Cleartext(message), hashPasswordResponse.Result, new RoundsExponent(4), CreateContext());
                if (encryptResponse.IsSuccess)
                {
                    var encodeResponse = _api.EncodeToVisualCryptText(encryptResponse.Result);
                    if (encodeResponse.IsSuccess)
                    {
                        visualCrypt = encodeResponse.Result.Text;
                    }
                    else
                        throw new Exception(encodeResponse.Error);
                }
                else
                    throw new Exception(encryptResponse.Error);

                // decrypt
                var decodeResponse = _api.TryDecodeVisualCryptText(visualCrypt);

                if (!decodeResponse.IsSuccess)
                    throw new Exception(decodeResponse.Error);

                var decryptResponse = _api.Decrypt(decodeResponse.Result, hashPasswordResponse.Result, CreateContext());

                Assert.IsTrue(decryptResponse.IsSuccess);
                Assert.IsTrue(decryptResponse.Result.Text.Equals(message), "The decrypted message does not equal the original message");
            }
        }



        [TestMethod]
        public void Cannot_Decrypt_With_Incorrect_Password()
        {
            foreach (var m in _strings)
            {
                var message = m + "message";
                var password = m + "password";

                var hashPasswordResponse = _api.CreateSHA512PW64(password);

                // encrypt
                string visualCrypt;
                var encryptResponse = _api.Encrypt(new Cleartext(message), hashPasswordResponse.Result, new RoundsExponent(4), CreateContext());
                if (encryptResponse.IsSuccess)
                {
                    var encodeResponse = _api.EncodeToVisualCryptText(encryptResponse.Result);
                    if (encodeResponse.IsSuccess)
                    {
                        visualCrypt = encodeResponse.Result.Text;
                    }
                    else
                        throw new Exception(encodeResponse.Error);
                }
                else
                    throw new Exception(encryptResponse.Error);

                // set incorrect password
                hashPasswordResponse = _api.CreateSHA512PW64("incorrect" + password);

                // decrypt
                var decodeResponse = _api.TryDecodeVisualCryptText(visualCrypt);

                if (!decodeResponse.IsSuccess)
                    throw new Exception(decodeResponse.Error);

                var decryptResponse = _api.Decrypt(decodeResponse.Result, hashPasswordResponse.Result, CreateContext());
                Assert.IsFalse(decryptResponse.IsSuccess);
                Assert.IsTrue(decryptResponse.Error == LocalizableStrings.MsgPasswordError, "Expected 'Password Error' when decrypting arbitrary data");
            }
        }

        [TestMethod]
        public void Member_EncodeToVisualCryptText()
        {
            var correctCipherV2 = CreateCipherV2("my password", "my message");

            VisualCryptText visualCryptText = _api.EncodeToVisualCryptText(correctCipherV2).Result;

            Assert.IsTrue(visualCryptText.Text.StartsWith("VisualCrypt/"));
            Assert.IsTrue(visualCryptText.Text.Length == 126);  // if the message fits one block lenght will be 126
        }

        [TestMethod]
        public void Member_EncodeToVisualCryptText_With_ArbitraryCipherV2()
        {
            var arbitraryCipherV2 = CreateArbitraryCipherV2();

            VisualCryptText visualCryptText = _api.EncodeToVisualCryptText(arbitraryCipherV2).Result;

            Assert.IsTrue(visualCryptText.Text.StartsWith("VisualCrypt/"));
            Assert.IsTrue(visualCryptText.Text.Length == 126); // if the message fits one block lenght will be 126
        }

        [TestMethod]
        public void Member_TryDecodeVisualCryptText()
        {
            // Sample: 'my message', 'my password', the variable visualCryptText contains also additional spaces, line break
            var visualCryptText = @"VisualCrypt/AgoESMEkbivp64nHLWWgRGoP6XuQsrwe9dIurkVbJy5RCLvT6KqhHCed775BSs
                                    zXvNlU1cxyGRnAzlnkxLHMJ1QNrC7jdoI+KCJKkJ98aDEF+pg=";

            var response = _api.TryDecodeVisualCryptText(visualCryptText);

            Assert.IsTrue(response.IsSuccess);
            CipherV2 cipherV2 = response.Result;

            var decrpytResponse = _api.Decrypt(cipherV2, CreatePasswordHash("my password"), CreateContext());
            Assert.IsTrue(decrpytResponse.IsSuccess);
            Assert.IsTrue(decrpytResponse.Result.Text.Equals("my message"));
        }


        [TestMethod]
        public void Member_TryDecodeVisualCryptText_Arbitrary()
        {
            var arbitraryCipherV2 = CreateArbitraryCipherV2();
            string arbitraryVisualCryptText = _api.EncodeToVisualCryptText(arbitraryCipherV2).Result.Text;

            var response = _api.TryDecodeVisualCryptText(arbitraryVisualCryptText);

            Assert.IsTrue(response.IsSuccess);

            var decrpytResponse = _api.Decrypt(response.Result, CreatePasswordHash("some password"), CreateContext());
            Assert.IsFalse(decrpytResponse.IsSuccess);
            Assert.IsTrue(decrpytResponse.Error.Equals(LocalizableStrings.MsgPasswordError));
        }

        [TestMethod]
        public void Member_TryDecodeVisualCryptText_Doesnt_Accept_Non_VisualCryptText()
        {
            var tests = new List<Response<CipherV2>>
            {
                // empty string
                _api.TryDecodeVisualCryptText(""),
                // just something
                _api.TryDecodeVisualCryptText("390214829374023"),
                // xyz after VisualCrypt/ is not valid
                _api.TryDecodeVisualCryptText("VisualCrypt/xyzoESMEkbivp64nHLWWgRGoP6XuQsrwe9dIurkVbJy5RCLvT6KqhHCed775BSszXvNlU1cxyGRnAzlnkxLHMJ1QNrC7jdoI+KCJKkJ98aDEF+pg="),
                // truncated, too short
                _api.TryDecodeVisualCryptText("VisualCrypt/AgoESMEkbivp64nHLWWgRGoP6XuQsrwe9dIurkVbJy5RCLvT6Kqh"),
                // invalid Base64 char 'ä' included
                 _api.TryDecodeVisualCryptText("VisualCrypt/AgoESMEkbivp64nHLWWgRGoP6XuQsrwe9dIurkVbJy5RCLvT6KqhHCedä775BSszXvNlU1cxyGRnAzlnkxLHMJ1QNrC7jdoI+KCJKkJ98aDEF+pg="),
                 // Chinese inserted in in otherwise correct format
                 _api.TryDecodeVisualCryptText("VisualCrypt/AgoESMEkbivp64nHLWWgRGoP6XuQs菲舍爾的弗里茨才對鮮魚rwe9dIurkVbJy5RCLvT6KqhHCed775BSszXvNlU1cxyGRnAzlnkxLHMJ1QNrC7jdoI+KCJKkJ98aDEF+pg="),
        };


            foreach (var test in tests)
            {
                Assert.IsFalse(test.IsSuccess);
                Assert.IsTrue(test.Error.StartsWith(LocalizableStrings.MsgFormatError));
            }
        }

        [TestMethod]
        public void Member_GenerateRandomPassword_Generates_256_Bit_Random_Passwords()
        {
            var sampleGeneratedPassword = "tdmkh pR990 stlWi f6VAi Jxblb\r\nAd55O gp1BI 0vJKh ujs";

            var tests = new List<Response<string>>();

            const decimal sampleSize = 1000m;

            for (int i = 1; i <= sampleSize; i++)
                tests.Add(_api.GenerateRandomPassword());

            foreach (var test in tests) // Assert general success and that the lenght is correct
            {
                Assert.IsTrue(test.IsSuccess);
                Assert.IsTrue(sampleGeneratedPassword.Length == 52);
                Assert.IsTrue(test.Result.Length == sampleGeneratedPassword.Length);
            }

            var passwordsAsBytes = new List<byte[]>();
            foreach (var generatedPassword in tests)
            {
                // convert back to standard Base64
                byte[] bytes = Base64Encoder.DecodeBase64StringToBinary(generatedPassword.Result.Replace("$", "/") + "=");
                passwordsAsBytes.Add(bytes);

                // Assert that we have 32 bytes of data, i.e. 256 Bit
                Assert.IsTrue(bytes.Length == 32);
            }
            // if the password data has random distribution, the expected sum should be near expectedTotal
            const decimal expectedTotal = sampleSize * 32m * 256m / 2m;

            decimal actualTotal = 0;
            foreach (var password in passwordsAsBytes)
                foreach (var byteValue in password)
                    actualTotal += byteValue;
            var result = actualTotal / expectedTotal;

            var closenessToTrueRandomInPercent = Math.Round(Math.Abs(1 - result) * 100, 4);
            Assert.IsTrue(closenessToTrueRandomInPercent != 0m, "That's nearly only possible when all numbers are the same.");

            Assert.IsTrue(closenessToTrueRandomInPercent < 1, "The generated passwords do not appear to be random.");
            Trace.WriteLine(string.Format("The quality of the 256 Bit random password deviated {0}% from true random data with the given sample size of {1}", closenessToTrueRandomInPercent, sampleSize));
        }


        [TestMethod]
        public void Member_PrunePassword()
        {
            const string expected = "Hello World";

            // New Line(s) -> single space
            const string input1 = "Hello\r\nWorld";
            Assert.AreEqual(expected, _api.PrunePassword(input1).Result.Text);

            const string input2 = "Hello\r\n\r\nWorld";
            Assert.AreEqual(expected, _api.PrunePassword(input2).Result.Text);

            const string input3 = "\nHello\n\r\n\nWorld\r\n";
            Assert.AreEqual(expected, _api.PrunePassword(input3).Result.Text);

            // Spaces -> single Space
            const string input4 = " \n  Hello\n\r\n\nWorld\r\n  ";
            Assert.AreEqual(expected, _api.PrunePassword(input4).Result.Text);

            // Remove Control Chars
            const string input5 = " \n\n\x01\x02\0Hello\n\r\n\nWorld\r\n  ";
            Assert.AreEqual(expected, _api.PrunePassword(input5).Result.Text);

            // Remove Control Chars with Spaces
            const string input6 = " \n\n\x01 \x02 \0 H\x01\x02\0ello\n\r\n\n \n\n\x01 \x02 \0 World \n\n\x01 \x02 \0 \r\n  ";
            Assert.AreEqual(expected, _api.PrunePassword(input6).Result.Text);

        }


        [TestMethod]
        public void Show_ControlChars_And_WhiteSpaceChars()
        {
            Assert.IsTrue(char.IsControl('\0'));
            Assert.IsTrue(char.IsControl('\x01'));
            Assert.IsTrue(char.IsControl('\x02'));

            Assert.IsTrue(!char.IsWhiteSpace('\0'));
            Assert.IsTrue(!char.IsWhiteSpace('\x01'));
            Assert.IsTrue(!char.IsWhiteSpace('\x02'));

            Assert.IsTrue(char.IsWhiteSpace('\r') && char.IsControl('\r'));
            Assert.IsTrue(char.IsWhiteSpace('\n') && char.IsControl('\n'));
        }



        SHA512PW64 CreatePasswordHash(string unprunedPassword)
        {
            var response = _api.CreateSHA512PW64(unprunedPassword);
            if (response.IsSuccess)
                return response.Result;
            throw new Exception(response.Error);
        }

        LongRunningOperationContext CreateContext()
        {
            return new LongRunningOperationContext(new CancellationToken(), new EncryptionProgress((progress) => { Trace.WriteLine(string.Format("Progress: {0} | {1}%", progress.Message, progress.Percent)); }));
        }

        CipherV2 CreateCipherV2(string password, string message)
        {
            var secret = new Cleartext(message);
            var passwordHash = CreatePasswordHash(password);
            var rounds = new RoundsExponent(4);
            var context = CreateContext();

            var response = _api.Encrypt(secret, passwordHash, rounds, context);
            if (response.IsSuccess)
                return response.Result;
            throw new Exception(response.Error);
        }

        CipherV2 CreateArbitraryCipherV2()
        {
            var arbitraryCipherV2 = new CipherV2  // create a CipherV2 object that passes parameter checks of its parts (not null, length, not all zero bytes)
            {
                IV16 = new IV16(new byte[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }),
                MACCipher16 = new MACCipher16(new byte[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }),
                MessageCipher = new MessageCipher(new byte[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }),
                RandomKeyCipher32 = new RandomKeyCipher32(new byte[32]
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
                    16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31
                }),
                Padding = new PlaintextPadding(0),
                RoundsExponent = new RoundsExponent(4)
            };
            return arbitraryCipherV2;
        }

        List<string> CreateStringsOfVariousLenghts()
        {
            var strings = new List<string>();
            var s = "";

            for (var i = 0; i <= 300; i++)
            {
                s += "1";
                strings.Add(s);
            }
            return strings;
        }
    }
}