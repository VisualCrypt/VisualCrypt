using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Storage;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.App.API.V2
{
	public class EncryptionService : IEncryptionService
	{
		readonly IVisualCrypt2API _visualCrypt2API;

		public EncryptionService()
		{
			_visualCrypt2API = new VisualCrypt2API(new Core2API());
		}

		RoundsExponent GetV2LogRoundsSetting()
		{
			return new RoundsExponent(10);
		}

		public Response<FileModel> OpenFile(string filename)
		{
			var response = new Response<FileModel>();

			try
			{
				if (filename == null)
					throw new ArgumentNullException("filename");

				// new in RT:
				StorageFile file = StorageFile.GetFileFromPathAsync(filename).GetResults();
				var buffer =  FileIO.ReadBufferAsync(file).GetResults();
				var rawBytesFromFile = buffer.ToArray();

				// old .net:
				// var rawBytesFromFile = File.ReadAllBytes(filename);

				Response<string, Encoding> getStringResponse = _visualCrypt2API.GetStringFromFile(rawBytesFromFile, null /* no Encoding.Default on platform */);

				if (!getStringResponse.IsSuccess) // we do not even have a string.
				{
					// in this case we return no FileModel,just return the error.
					response.SetError(getStringResponse.Error);
					return response;
				}

				// if we are here we have a string. Is it VisualCrypt/text or just Cleartext?
				var decodeResponse = _visualCrypt2API.TryDecodeVisualCryptText(getStringResponse.Result);

				if (decodeResponse.IsSuccess)
				{
					// it's VisualCrypt!
					var encryptedFileModel = FileModel.Encrypted(decodeResponse.Result, filename, getStringResponse.Result);
					response.Result = encryptedFileModel;
				}
				else
				{
					// it's Cleartext
					var cleartextFileModel = FileModel.Cleartext(filename, getStringResponse.Result, getStringResponse.Result2);
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

		public Response<FileModel> EncryptForDisplay(FileModel fileModel, string textBufferContents, LongRunningOperationContext context)
		{
			var response = new Response<FileModel>();

			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (fileModel.IsEncrypted)
					throw new InvalidOperationException("IsEncrypted is already true - not allowed here.");

				var encryptResponse = _visualCrypt2API.Encrypt(new Cleartext(textBufferContents), KeyStore.GetSHA256PW32(), GetV2LogRoundsSetting(), context);
				context.CancellationToken.ThrowIfCancellationRequested();
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCrypt2API.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						CipherV2 cipherV2 = encryptResponse.Result;
						var encryptedFileModel = FileModel.Encrypted(cipherV2, fileModel.Filename, visualCryptText.Text);
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
				Response<SHA512PW64> sha512PW64Response = _visualCrypt2API.CreateSHA512PW64(unprunedUTF16LEPassword);
				if (sha512PW64Response.IsSuccess)
				{
					KeyStore.SetSHA256PW32(sha512PW64Response.Result);
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

				var decodeResponse = _visualCrypt2API.TryDecodeVisualCryptText(textBufferContents);
				if (decodeResponse.IsSuccess)
				{
					var decrpytResponse = _visualCrypt2API.Decrypt(decodeResponse.Result, KeyStore.GetSHA256PW32(), context);
					if (decrpytResponse.IsSuccess)
					{
						Cleartext cleartext = decrpytResponse.Result;
						var clearTextFileModel = FileModel.Cleartext(fileModel.Filename, cleartext.Text, fileModel.SaveEncoding);
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

				if (!_visualCrypt2API.TryDecodeVisualCryptText(fileModel.VisualCryptText).IsSuccess)
					throw new Exception("Aborting Save -  The data being saved is not valid VisualCrypt format.");

				byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(fileModel.VisualCryptText);

				throw new NotImplementedException();
				//File.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);
				response.SetSuccess();
			}
			catch (Exception e)
			{
				response.SetError(e);
			}
			return response;
		}


		public Response<string> EncryptAndSaveFile(FileModel fileModel, string textBufferContents, LongRunningOperationContext context)
		{
			var response = new Response<string>();

			try
			{
				if (fileModel == null)
					throw new ArgumentNullException("fileModel");

				if (fileModel.IsEncrypted)
					throw new InvalidOperationException("IsEncrypted is already true - not allowed here.");

				var encryptResponse = _visualCrypt2API.Encrypt(new Cleartext(textBufferContents), KeyStore.GetSHA256PW32(), GetV2LogRoundsSetting(), context);
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCrypt2API.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(visualCryptText.Text);
						
						throw new NotImplementedException();
						//File.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);

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
			return _visualCrypt2API.GenerateRandomPassword();
		}


		public Response<string> SanitizePassword(string unsanitizedPassword)
		{
			var response = new Response<string>();
			try
			{
				var sanitizePasswordResponse = _visualCrypt2API.PrunePassword(unsanitizedPassword);
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