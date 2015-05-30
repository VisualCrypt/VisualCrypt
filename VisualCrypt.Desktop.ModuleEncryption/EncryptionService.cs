using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using VisualCrypt.Cryptography.Net.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.ModuleEncryption
{
	[Export(typeof (IEncryptionService))]
	public class EncryptionService : IEncryptionService
	{
		readonly IVisualCryptAPIV2 _visualCryptApiv2;

		public EncryptionService()
		{
			_visualCryptApiv2 = new VisualCryptAPIV2(new CoreAPIV2_Net4());
		}

		public Response<FileModelBase> OpenFile(string filename)
		{
			var response = new Response<FileModelBase>();

			try
			{
				if (filename == null)
					throw new ArgumentNullException("filename");

				var rawBytesFromFile = File.ReadAllBytes(filename);

				Response<string, Encoding> getStringResponse = _visualCryptApiv2.GetStringFromFileBytes(rawBytesFromFile,
					Encoding.Default);

				if (!getStringResponse.Success) // we do not even have a string.
				{
					// in this case we return no FileModelBase,just return the error.
					response.Error = getStringResponse.Error;
					return response;
				}

				// if we are here we have a string. Is it VisualCrypt/text or just Cleartext?
				var decodeResponse = _visualCryptApiv2.TryDecodeVisualCryptText(getStringResponse.Result);

				if (decodeResponse.Success)
				{
					// it's VisualCrypt!
					var encryptedFileModel = new EncryptedFileModel(getStringResponse.Result, decodeResponse.Result, filename);
					response.Result = encryptedFileModel;
				}
				else
				{
					// it's ClearText
					var cleartextFileModel = new CleartextFileModel(getStringResponse.Result, getStringResponse.Result2,
						filename);
					response.Result = cleartextFileModel;
				}
				response.Success = true;
			}
			catch (Exception e)
			{
				response.Error = e.Message;
			}
			return response;
		}

		public Response SetPassword(byte[] utf16LEPassword)
		{
			var response = new Response();
			try
			{
				var sha256Response = _visualCryptApiv2.CreateSHA256PW32(utf16LEPassword);
				if (sha256Response.Success)
				{
					KeyStore.SetSHA256PW32(sha256Response.Result);
					response.Success = true;
				}
				else
					response.Error = sha256Response.Error;
			}
			catch (Exception e)
			{
				response.Error = e.Message;
			}
			return response;
		}

		public Response ClearPassword()
		{
			var response = new Response();
			try
			{
				KeyStore.Clear();
				response.Success = true;
			}
			catch (Exception e)
			{
				response.Error = e.Message;
			}
			return response;
		}
	}
}