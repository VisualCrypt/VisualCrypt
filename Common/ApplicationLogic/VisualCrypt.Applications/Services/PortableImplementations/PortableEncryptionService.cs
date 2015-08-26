using System;
using System.Text;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Cryptography.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

namespace VisualCrypt.Applications.Services.PortableImplementations
{
	public class PortableEncryptionService : IEncryptionService
	{
		readonly IVisualCrypt2Service _visualCrypt2Service;
	    readonly IEncodingDetection _encodingDetection;
	    readonly IFileService _fileService;

	    public PortableEncryptionService()
	    {
	        _visualCrypt2Service = Service.Get<IVisualCrypt2Service>();
	        _encodingDetection = Service.Get<IEncodingDetection>();
	        _fileService = Service.Get<IFileService>();
	    }


		public Response<FileModel> OpenFile(string filename)
		{
			var response = new Response<FileModel>();

			try
			{
				if (filename == null)
					throw new ArgumentNullException("filename");

                var rawBytesFromFile = _fileService.ReadAllBytes(filename);
			    var shortFilename = _fileService.PathGetFileName(filename);

				Response<string, Encoding> getStringResponse = _encodingDetection.GetStringFromFile(rawBytesFromFile);

				if (!getStringResponse.IsSuccess) // we do not even have a string.
				{
					// in this case we return no FileModel,just return the error.
					response.SetError(getStringResponse.Error);
					return response;
				}

				// if we are here we have a string. Is it VisualCrypt/text or just Cleartext?
				var decodeResponse = _visualCrypt2Service.DecodeVisualCrypt(getStringResponse.Result);

				if (decodeResponse.IsSuccess)
				{
					// it's VisualCrypt!
					var encryptedFileModel = FileModel.Encrypted(decodeResponse.Result, filename, shortFilename, getStringResponse.Result);
					response.Result = encryptedFileModel;
				}
				else
				{
					// it's Cleartext
					var cleartextFileModel = FileModel.Cleartext(filename, shortFilename, getStringResponse.Result, getStringResponse.Result2);
					response.Result = cleartextFileModel;
				}
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response<FileModel> EncryptForDisplay(FileModel fileModel, string textBufferContents, RoundsExponent roundsExponent, LongRunningOperationContext context)
		{
			var response = new Response<FileModel>();

			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (fileModel.IsEncrypted)
					throw new InvalidOperationException("IsEncrypted is already true - not allowed here.");

				var encryptResponse = _visualCrypt2Service.Encrypt(new Cleartext(textBufferContents), KeyStore.GetPasswordHash(), roundsExponent, context);
				context.CancellationToken.ThrowIfCancellationRequested();
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCrypt2Service.EncodeVisualCrypt(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						CipherV2 cipherV2 = encryptResponse.Result;
						var encryptedFileModel = FileModel.Encrypted(cipherV2, fileModel.Filename, fileModel.ShortFilename,visualCryptText.Text);
						encryptedFileModel.IsDirty = fileModel.IsDirty; // preserve IsDirty
						response.Result = encryptedFileModel;
						response.SetSuccess();
					}
					else response.SetError(encodeResponse.Error);
				}
				else response.SetError(encryptResponse.Error);
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response SetPassword(string unprunedUTF16LEPassword)
		{
			var response = new Response();
			try
			{
				Response<SHA512PW64> sha512PW64Response = _visualCrypt2Service.HashPassword(_visualCrypt2Service.NormalizePassword(unprunedUTF16LEPassword).Result);
				if (sha512PW64Response.IsSuccess)
				{
					KeyStore.SetPasswordHash(sha512PW64Response.Result);
					response.SetSuccess();
				}
				else
					response.SetError(sha512PW64Response.Error);
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response ClearPassword()
		{
			var response = new Response();
			try
			{
				KeyStore.Clear();
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response<FileModel> DecryptForDisplay(FileModel fileModel, string textBufferContents, LongRunningOperationContext context)
		{
			var response = new Response<FileModel>();
			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (textBufferContents == null)
					throw new ArgumentNullException("textBufferContents");

				if (context == null)
					throw new ArgumentNullException("context");

				var decodeResponse = _visualCrypt2Service.DecodeVisualCrypt(textBufferContents);
				if (decodeResponse.IsSuccess)
				{
					var decrpytResponse = _visualCrypt2Service.Decrypt(decodeResponse.Result, KeyStore.GetPasswordHash(), context);
					if (decrpytResponse.IsSuccess)
					{
						Cleartext cleartext = decrpytResponse.Result;
						var clearTextFileModel = FileModel.Cleartext(fileModel.Filename, fileModel.ShortFilename, cleartext.Text, fileModel.SaveEncoding);
						clearTextFileModel.IsDirty = fileModel.IsDirty; // preserve IsDirty
						response.Result = clearTextFileModel;
						response.SetSuccess();
					}
					else response.SetError(decrpytResponse.Error);
				}
				else response.SetError(decodeResponse.Error);
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response SaveEncryptedFile(FileModel fileModel)
		{
			var response = new Response();

			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (!fileModel.IsEncrypted)
					throw new Exception("Aborting Save - IsEncrypted is false.");

				if (!_visualCrypt2Service.DecodeVisualCrypt(fileModel.VisualCryptText).IsSuccess)
					throw new Exception("Aborting Save -  The data being saved is not valid VisualCrypt format.");

				byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(fileModel.VisualCryptText);
				_fileService.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		public Response<string> EncryptAndSaveFile(FileModel fileModel, string textBufferContents, RoundsExponent roundsExponent, LongRunningOperationContext context)
		{
			var response = new Response<string>();

			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (fileModel.IsEncrypted)
					throw new InvalidOperationException("IsEncrypted is already true - not allowed here.");

				var encryptResponse = _visualCrypt2Service.Encrypt(new Cleartext(textBufferContents), KeyStore.GetPasswordHash(), roundsExponent, context);
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCrypt2Service.EncodeVisualCrypt(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(visualCryptText.Text);
						_fileService.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);
						response.Result = visualCryptText.Text;
						response.SetSuccess();
					}
					else response.SetError(encodeResponse.Error);
				}
				else response.SetError(encryptResponse.Error);
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}

		public Response<string> GenerateRandomPassword()
		{
			return _visualCrypt2Service.SuggestRandomPassword();
		}


		public Response<string> SanitizePassword(string unsanitizedPassword)
		{
			var response = new Response<string>();
			try
			{
				var sanitizePasswordResponse = _visualCrypt2Service.NormalizePassword(unsanitizedPassword);
				if (sanitizePasswordResponse.IsSuccess)
				{
					response.Result = sanitizePasswordResponse.Result.Text;
					response.SetSuccess();
				}
				else
				{
					response.SetError(sanitizePasswordResponse.Error);
				}
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;

		}
	}
}