using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Net.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Desktop.Shared.Files
{
    public class VisualCryptFileModel : FileModelBase
    {
        readonly Encoding VisualCryptSaveEncoding = new UTF8Encoding(false,true);
        IVisualCryptAPIV2 _api = new VisualCryptAPIV2(new CoreAPIV2_Net4());

 

        bool _isPasswordPresent = false;
        public bool IsPasswordPresent { get { return _isPasswordPresent; } }

        bool _isFilenamePresent = false;
        public bool IsFilenamePresent { get { return _isFilenamePresent; } }

      

        VisualCryptText _visualCryptText = null;
        public VisualCryptText VisualCryptText { get { return _visualCryptText; } }

        SHA256PW32 _sha256PW32 = null;

        public Encoding SaveEncoding { get { return new UTF8Encoding(true, false); } }

       
        public void SetFilename(string filename)
        {
            Filename = filename;
            _isFilenamePresent = true;
        }

        public Response<SHA256PW32> SetPassword(byte[] utf16LEPassword)
        {
            var response = _api.CreateSHA256PW32(utf16LEPassword);
            if (response.Success)
            {
                _sha256PW32 = response.Result;
                _isPasswordPresent = true;
            }
            return response;
        }

        public async Task<Response<VisualCryptText>> Encrypt(ClearText clearText)
        {

            // TODO: check the whole state
            var encrpytResponse = _api.Encrypt(clearText, _sha256PW32);
            if (encrpytResponse.Success)
            {
                var response2 = _api.EncodeToVisualCryptText(encrpytResponse.Result);
                if (response2.Success)
                {
                    IsEncrypted = true;
                    _visualCryptText = response2.Result;
                }
                return response2;
            }
            return new Response<VisualCryptText> { Error = encrpytResponse.Error };
        }

        public Response<ClearText> Decrypt(CipherV2 cipherV2)
        {
            var response = new Response<ClearText>();
            try
            {
                if (cipherV2 == null)
                    throw new ArgumentNullException("cipherV2");

                var decryptResponse = _api.Decrpyt(cipherV2, _sha256PW32);
                if (decryptResponse.Success)
                {
                    IsEncrypted = false;
                    response.Result = decryptResponse.Result;
                    response.Success = true;
                }
                else
                    response.Error = decryptResponse.Error;
            }
            catch (Exception e)
            {
                response.Error = e.Message;
            }
            return response;
        }

        #region Functions that set the State to ENCRYPTED

        public Response<string> TryLoadVisualCryptTextOrCleartext()
        {
            var response = new Response<string>();

            try
            {
                if (Filename == null)
                    throw new InvalidOperationException("_filename must not be null.");

                var rawBytesFromFile = File.ReadAllBytes(Filename);

                var getStringResponse = _api.GetStringFromFileBytes(rawBytesFromFile, Encoding.Default);

                if (!getStringResponse.Success)  // we do not even have a string.
                {
                    // in this case we do no changes in VisualCryptFileModel and just return the error.
                    response.Error = getStringResponse.Error;
                    return response;
                }

                // if we are here we have a string. Is it VisualCrypt/text or just Cleartext?
                var decodeResponse = _api.TryDecodeVisualCryptText(getStringResponse.Result);

                if (decodeResponse.Success)
                {
                    // it's VisualCrypt
                    _visualCryptText = new VisualCryptText(getStringResponse.Result);
                    IsEncrypted = true;
                }
                else
                {
                    // it's ClearText
                    IsEncrypted = false;
                }
                response.Result = getStringResponse.Result;
                response.Success = true;

            }
            catch (Exception e)
            {
                response.Error = e.Message;
            }
            return response;

        }

        public bool InitFromVisualCryptString(string pastedVisualCryptText)
        {
            //         if VisualCrypt.API.IsVisualCrypt(visualCryptString) {
            //        _encryptedContents = visualCryptString
            //        _isEncrypted = true
            //        return true
            //    }
            //_isEncrypted = false
            return false;
        }

        #endregion

        public Response SaveEncrypted()
        {
            Thread.Sleep(2000);
            var response = new Response();
            try
            {
                if (!IsEncrypted)
                    throw new Exception("Aborting Save - _isEncrypted was unexpectedly false.");
                if (!_isFilenamePresent)
                    throw new Exception("Aborting Save - _isFilenamePresent was unexpectedly false.");
                if (!_api.TryDecodeVisualCryptText(_visualCryptText.Value).Success)
                    throw new Exception(
                        "Aborting Save -  the data being saved is unexpectedly not in valid VisualCrypt format.");

                byte[] visualCryptTextBytes = VisualCryptSaveEncoding.GetBytes(_visualCryptText.Value);
                File.WriteAllBytes(Filename, visualCryptTextBytes);
                response.Success = true;
            }
            catch (Exception e)
            {
                response.Error = e.Message;
            }
            return response;
        }

        internal void ClearPassword()
        {
            if (_sha256PW32 != null)
                _sha256PW32.Value.OverwriteWithZeros();
            _isPasswordPresent = false;
        }

        internal void NotifyRedisplayingClearText()
        {
            IsEncrypted = false;
        }
    }

    
}
