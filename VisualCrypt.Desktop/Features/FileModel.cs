using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using VisualCrypt.Net.APIV2.Implementations;
using VisualCrypt.Portable.APIV2.DataTypes;
using VisualCrypt.Portable.APIV2.Implementations;
using VisualCrypt.Portable.APIV2.Interfaces;
using VisualCrypt.Portable.Tools;

namespace VisualCrypt.Desktop.Features
{
    public class FileModel
    {
        IVisualCryptAPIV2 _api = new VisualCryptAPIV2(new CoreAPIV2_Net4());

        bool _isEncrypted = false;
        public bool IsEncrypted { get { return _isEncrypted; } }

        bool _isPasswordPresent = false;
        public bool IsPasswordPresent { get { return _isPasswordPresent; } }

        bool _isFilenamePresent = false;
        public bool IsFilenamePresent { get { return _isFilenamePresent; } }

        string _filename = string.Empty;
        public string Filename { get { return _filename; } }

        VisualCryptText _visualCryptText = null;
        public VisualCryptText VisualCryptText { get { return _visualCryptText; } }

        SHA256PW32 _sha256PW32 = null;

        public Encoding SaveEncoding { get { return new UTF8Encoding(true, false); } }

        public bool IsDirty { get; set; }
        public void SetFilename(string filename)
        {
            _filename = filename;
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

        public Response<VisualCryptText> Encrypt(ClearText clearText)
        {
            // TODO: check the whole state
            var encrpytResponse = _api.Encrypt(clearText, _sha256PW32);
            if (encrpytResponse.Success)
            {
                var response2 = _api.EncodeToVisualCryptText(encrpytResponse.Result);
                if (response2.Success)
                {
                    _isEncrypted = true;
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
                    _isEncrypted = false;
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
                if (_filename == null)
                    throw new InvalidOperationException("_filename must not be null.");

                var rawBytesFromFile = File.ReadAllBytes(_filename);

                var getStringResponse = _api.GetStringFromFileBytes(rawBytesFromFile, Encoding.Default);

                if (!getStringResponse.Success)  // we do not even have a string.
                {
                    // in this case we do no changes in FileModel and just return the error.
                    response.Error = getStringResponse.Error;
                    return response;
                }

                // if we are here we have a string. Is it VisualCrypt/text or just Cleartext?
                var decodeResponse = _api.TryDecodeVisualCryptText(new VisualCryptText(getStringResponse.Result));

                if (decodeResponse.Success)
                {
                    // it's VisualCrypt
                    _visualCryptText = new VisualCryptText(getStringResponse.Result);
                    _isEncrypted = true;
                }
                else
                {
                    // it's ClearText
                    _isEncrypted = false;
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

        public void SaveEncrypted()
        {
            // here is the place to ensure 100% that it is really encrypted;
            Debug.Assert(_isEncrypted);
            Debug.Assert(_isFilenamePresent);
            if (!_api.TryDecodeVisualCryptText(_visualCryptText).Success)
                throw new Exception("Bug");
            File.WriteAllText(_filename, _visualCryptText.Value);
        }

        internal void ClearPassword()
        {
            if (_sha256PW32 != null)
                _sha256PW32.Value.OverwriteWithZeros();
            _isPasswordPresent = false;
        }
    }

    
}
