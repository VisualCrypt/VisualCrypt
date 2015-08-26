using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.Cryptography.VisualCrypt2.Implementations
{
    public class VisualCrypt2Service : IVisualCrypt2Service
    {
        VisualCryptAPI2Internal _internal;
        IPlatform _platform;


        public IPlatform Platform
        {
            set
            {
                if (_platform != null)
                    throw new InvalidOperationException("An instance of IPlatform has already been supplied.");
                _platform = value;
                _internal = new VisualCryptAPI2Internal(_platform);
            }
        }
      

        public Response<SHA512PW64> HashPassword(NormalizedPassword normalizedPassword)
        {
            var response = new Response<SHA512PW64>();

            try
            {
                Guard.NotNull(normalizedPassword);
                EnsurePlatform();

                var utf16LEBytes = Encoding.Unicode.GetBytes(normalizedPassword.Text);

                var sha512 = _platform.ComputeSHA512(utf16LEBytes);

                response.Result = new SHA512PW64(sha512);
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }


        public Response<CipherV2> Encrypt(Cleartext cleartext, SHA512PW64 sha512PW64, RoundsExponent roundsExponent,
            LongRunningOperationContext context)
        {
            var response = new Response<CipherV2>();

            try
            {
                Guard.NotNull(new object[] { cleartext, sha512PW64, roundsExponent, context });
                EnsurePlatform();

                Compressed compressed = _internal.Compress(cleartext);

                PaddedData paddedData = _internal.ApplyRandomPadding(compressed);

                IV16 iv = new IV16(_platform.GenerateRandomBytes(16));

                PasswordDerivedKey32 passwordDerivedKey = CreatePasswordDerivedKey(iv, sha512PW64, roundsExponent, context);

                RandomKey32 randomKey = new RandomKey32(_platform.GenerateRandomBytes(32));

                var cipherV2 = new CipherV2 { RoundsExponent = roundsExponent, IV16 = iv };
                _internal.AESEncryptRandomKeyWithPasswordDerivedKey(passwordDerivedKey, randomKey, cipherV2, context);

                _internal.AESEncryptMessageWithRandomKey(paddedData, randomKey, cipherV2, context);

                MAC16 mac = CreateMAC(cipherV2, context);

                _internal.AESEncryptMACWithRandomKey(cipherV2, mac, randomKey, context);

                response.Result = cipherV2;
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }


        static MAC16 CreateMAC(CipherV2 cipherV2, LongRunningOperationContext context)
        {


            // Create the MAC only for items that, while decrypting, have not been used up to this point but do include the version.
            var securables = ByteArrays.Concatenate(cipherV2.MessageCipher.GetBytes(), new[] { cipherV2.Padding.ByteValue },
                new[] { CipherV2.Version });

            context.EncryptionProgress.Message = LocalizableStrings.MsgCalculatingMAC;
            BCrypt24 slowMAC = BCrypt.CreateHash(cipherV2.IV16, securables, cipherV2.RoundsExponent.Value, context);

            // See e.g. http://csrc.nist.gov/publications/fips/fips180-4/fips-180-4.pdf Chapter 7 for hash truncation.
            var truncatedMAC = new byte[16];
            Buffer.BlockCopy(slowMAC.GetBytes(), 0, truncatedMAC, 0, 16);
            return new MAC16(truncatedMAC);
        }


        PasswordDerivedKey32 CreatePasswordDerivedKey(IV16 iv, SHA512PW64 sha512PW64, RoundsExponent roundsExponent,
            LongRunningOperationContext context)
        {

            var leftSHA512 = new byte[32];
            var rightSHA512 = new byte[32];
            Buffer.BlockCopy(sha512PW64.GetBytes(), 0, leftSHA512, 0, 32);
            Buffer.BlockCopy(sha512PW64.GetBytes(), 32, rightSHA512, 0, 32);

            context.EncryptionProgress.Message = LocalizableStrings.MsgProcessingKey;

            // Compute the left side on a ThreadPool thread
            var task = Task.Run(() => BCrypt.CreateHash(iv, leftSHA512, roundsExponent.Value, context));

            // Compute the right side after dispatching the work for the right side
            BCrypt24 rightBCrypt = BCrypt.CreateHash(iv, rightSHA512, roundsExponent.Value, context);

            // Wait for the left side result
            task.Wait(context.CancellationToken);

            // Use the results
            var combinedHashes = ByteArrays.Concatenate(sha512PW64.GetBytes(), task.Result.GetBytes(), rightBCrypt.GetBytes());
            Debug.Assert(combinedHashes.Length == 64 + 24 + 24);

            var condensedHash = _platform.ComputeSHA256(combinedHashes);
            return new PasswordDerivedKey32(condensedHash);
        }


        public Response<VisualCryptText> EncodeVisualCrypt(CipherV2 cipherV2)
        {
            var response = new Response<VisualCryptText>();

            try
            {
                Guard.NotNull(cipherV2);
                EnsurePlatform();

                response.Result = VisualCrypt2Formatter.CreateVisualCryptText(cipherV2);
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }

        public Response<CipherV2> DecodeVisualCrypt(string visualCryptText)
        {
            var response = new Response<CipherV2>();

            try
            {
                Guard.NotNull(visualCryptText);
                EnsurePlatform();

                response.Result = VisualCrypt2Formatter.DissectVisualCryptText(visualCryptText);
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }


        public Response<Cleartext> Decrypt(CipherV2 cipherV2, SHA512PW64 sha512PW64, LongRunningOperationContext context)
        {
            var response = new Response<Cleartext>();

            try
            {
                Guard.NotNull(new object[] { cipherV2, sha512PW64, context });
                EnsurePlatform();

                PasswordDerivedKey32 passwordDerivedKey = CreatePasswordDerivedKey(cipherV2.IV16, sha512PW64, cipherV2.RoundsExponent,
                    context);

                RandomKey32 randomKey = _internal.AESDecryptRandomKeyWithPasswordDerivedKey(cipherV2, passwordDerivedKey, context);

                MAC16 decryptedMAC = _internal.AESDecryptMAC(cipherV2, randomKey, context);

                MAC16 actualMAC = CreateMAC(cipherV2, context);

                if (!actualMAC.GetBytes().SequenceEqual(decryptedMAC.GetBytes()))
                {
                    response.SetError(LocalizableStrings.MsgPasswordError);
                    return response;
                }


                PaddedData paddedData = _internal.AESDecryptMessage(cipherV2, cipherV2.IV16, randomKey, context);

                Compressed compressed = _internal.RemovePadding(paddedData);

                Cleartext cleartext = _internal.Decompress(compressed);

                response.Result = cleartext;
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }


        public Response<string> SuggestRandomPassword()
        {
            var response = new Response<string>();
            try
            {
                EnsurePlatform();
                response.Result = _internal.GenerateRandomPassword();
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }

        /// <summary>
        /// Removes all leading and trailing Unicode whitespace characters and replaces the remaining whitespace characters
        /// with u\0020 space characters. Adjacent whitespace is condensed to a single u\0020 character. Other Unicode
        /// control characters are stripped completely.
        /// The control characters are specifically the Unicode values U+0000 to U+001F and U+007F to U+009F;
        /// whitespace characters as defined by Char.IsWhiteSpace in .net 4.5.
        /// </summary>
        /// <param name="rawPassword">The password string obtained from textBox.Text.</param>
        /// <returns>The sanitized UTF-16 password string, the bytes of which are used as input for the password hash function.</returns>
        /// <see cref="http://www.unicode.org/Public/UNIDATA/UnicodeData.txt"/>
        public Response<NormalizedPassword> NormalizePassword(string rawPassword)
        {
            var response = new Response<NormalizedPassword>();
            try
            {
                Guard.NotNull(rawPassword);
                EnsurePlatform();


                // from msdn: White-space characters are defined by the Unicode standard. 
                // The Trim() method removes any leading and trailing characters that produce 
                // a return value of true when they are passed to the Char.IsWhiteSpace method.
                string sanitized =
                    rawPassword
                        .FilterNonWhitespaceControlCharacters()
                        .CondenseAndNormalizeWhiteSpace().
                        Trim();

                response.Result = new NormalizedPassword(sanitized);
                response.SetSuccess();
            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return response;
        }

        void EnsurePlatform()
        {
            if (_platform == null || _internal == null)
                throw new InvalidOperationException("You must supply an instance of IPlatform before you can use the service.");
        }
    }
}