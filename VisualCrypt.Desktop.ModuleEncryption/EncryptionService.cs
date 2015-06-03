using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Net.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;
using VisualCrypt.Desktop.Shared.Files;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.ModuleEncryption
{
	[Export(typeof(IEncryptionService))]
	public class EncryptionService : IEncryptionService
	{
		readonly IVisualCryptAPIV2 _visualCryptApiv2;

		public EncryptionService()
		{
			_visualCryptApiv2 = new VisualCryptAPIV2(new CoreAPIV2_Net4());
		}

		public Response<FileModel> OpenFile(string filename)
		{
			var response = new Response<FileModel>();

			try
			{
				if (filename == null)
					throw new ArgumentNullException("filename");

				var rawBytesFromFile = File.ReadAllBytes(filename);

				Response<string, Encoding> getStringResponse = _visualCryptApiv2.GetStringFromFileBytes(rawBytesFromFile,
					Encoding.Default);

				if (!getStringResponse.Success) // we do not even have a string.
				{
					// in this case we return no FileModel,just return the error.
					response.Error = getStringResponse.Error;
					return response;
				}

				// if we are here we have a string. Is it VisualCrypt/text or just Cleartext?
				var decodeResponse = _visualCryptApiv2.TryDecodeVisualCryptText(getStringResponse.Result);

				if (decodeResponse.Success)
				{
					// it's VisualCrypt!
					var encryptedFileModel = FileModel.Encrypted(decodeResponse.Result, filename, getStringResponse.Result);
					response.Result = encryptedFileModel;
				}
				else
				{
					// it's ClearText
					var cleartextFileModel = FileModel.Cleartext(filename, getStringResponse.Result, getStringResponse.Result2);
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

		public Response<FileModel> EncryptForDisplay(FileModel fileModel, string textBufferContents)
		{
			var response = new Response<FileModel>();

			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (fileModel.IsEncrypted)
					throw new InvalidOperationException("IsEncrypted is already true - not allowed here.");

				var encryptResponse = _visualCryptApiv2.Encrypt(new ClearText(textBufferContents), KeyStore.GetSHA256PW32());
				if (encryptResponse.Success)
				{
					var encodeResponse = _visualCryptApiv2.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.Success)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						CipherV2 cipherV2 = encryptResponse.Result;
						var encryptedFileModel = FileModel.Encrypted(cipherV2, fileModel.Filename, visualCryptText.Value);
						encryptedFileModel.IsDirty = fileModel.IsDirty; // preserve IsDirty
						response.Result = encryptedFileModel;
						response.Success = true;
					}
					else response.Error = encodeResponse.Error;
				}
				else response.Error = encryptResponse.Error;
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

		public Response<FileModel> DecryptForDisplay(FileModel fileModel, string textBufferContents)
		{
			var response = new Response<FileModel>();
			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (textBufferContents == null)
					throw new ArgumentNullException("textBufferContents");

				var decodeResponse = _visualCryptApiv2.TryDecodeVisualCryptText(textBufferContents);
				if (decodeResponse.Success)
				{
					var decrpytResponse = _visualCryptApiv2.Decrypt(decodeResponse.Result, KeyStore.GetSHA256PW32());
					if (decrpytResponse.Success)
					{
						ClearText clearText = decrpytResponse.Result;
						var clearTextFileModel = FileModel.Cleartext(fileModel.Filename, clearText.Value, fileModel.SaveEncoding);
						clearTextFileModel.IsDirty = fileModel.IsDirty; // preserve IsDirty
						response.Result = clearTextFileModel;
						response.Success = true;
					}
					else response.Error = decrpytResponse.Error;
				}
				else response.Error = decodeResponse.Error;
			}
			catch (Exception e)
			{
				response.Error = e.Message;
			}
			return response;
		}

		public Response SaveEncryptedFile(FileModel fileModel)
		{
			var response = new Response();
			
			try
			{
				if(fileModel == null)
					throw new ArgumentNullException("fileModel");
				
				if (!fileModel.IsEncrypted)
					throw new Exception("Aborting Save - IsEncrypted is false.");
				
				if (!_visualCryptApiv2.TryDecodeVisualCryptText(fileModel.VisualCryptText).Success)
					throw new Exception("Aborting Save -  The data being saved is not valid VisualCrypt format.");

				byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(fileModel.VisualCryptText);
				File.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);
				response.Success = true;
			}
			catch (Exception e)
			{
				response.Error = e.Message;
			}
			return response;
		}


		public Response<string> EncryptAndSaveFile(FileModel fileModel, string textBufferContents)
		{
			var response = new Response<string>();

			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (fileModel.IsEncrypted)
					throw new InvalidOperationException("IsEncrypted is already true - not allowed here.");

				var encryptResponse = _visualCryptApiv2.Encrypt(new ClearText(textBufferContents), KeyStore.GetSHA256PW32());
				if (encryptResponse.Success)
				{
					var encodeResponse = _visualCryptApiv2.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.Success)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(visualCryptText.Value);
						File.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);
						response.Result = visualCryptText.Value;
						response.Success = true;
					}
					else response.Error = encodeResponse.Error;
				}
				else response.Error = encryptResponse.Error;
			}
			catch (Exception e)
			{
				response.Error = e.Message;
			}
			return response;
		}
	}
}