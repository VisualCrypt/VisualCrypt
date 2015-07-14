using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using VisualCrypt.Cryptography.Net.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;
using VisualCrypt.Cryptography.Portable.APIV2.Interfaces;
using VisualCrypt.Desktop.Shared.App;
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

		BWF GetV2LogRoundsSetting()
		{
			return new BWF(SettingsManager.EditorSettings.CryptographySettings.LogRounds);
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

				if (!getStringResponse.IsSuccess) // we do not even have a string.
				{
					// in this case we return no FileModel,just return the error.
					response.SetError(getStringResponse.Error);
					return response;
				}

				// if we are here we have a string. Is it VisualCrypt/text or just Cleartext?
				var decodeResponse = _visualCryptApiv2.TryDecodeVisualCryptText(getStringResponse.Result);

				if (decodeResponse.IsSuccess)
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

				var encryptResponse = _visualCryptApiv2.Encrypt2(new ClearText(textBufferContents), KeyStore.GetSHA256PW32(), GetV2LogRoundsSetting(), context.Progress, context.CancellationToken);
				context.CancellationToken.ThrowIfCancellationRequested();
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCryptApiv2.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						CipherV2 cipherV2 = encryptResponse.Result;
						var encryptedFileModel = FileModel.Encrypted(cipherV2, fileModel.Filename, visualCryptText.Value);
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

		public Response SetPassword(string unsanitizedUTF16LEPassword)
		{
			var response = new Response();
			try
			{
				var sha256Response = _visualCryptApiv2.CreateSHA256PW32(unsanitizedUTF16LEPassword);
				if (sha256Response.IsSuccess)
				{
					KeyStore.SetSHA256PW32(sha256Response.Result);
					response.SetSuccess();
				}
				else
					response.SetError(sha256Response.Error);
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

				var decodeResponse = _visualCryptApiv2.TryDecodeVisualCryptText(textBufferContents);
				if (decodeResponse.IsSuccess)
				{
					var decrpytResponse = _visualCryptApiv2.Decrypt2(decodeResponse.Result, KeyStore.GetSHA256PW32(), context.Progress, context.CancellationToken);
					if (decrpytResponse.IsSuccess)
					{
						ClearText clearText = decrpytResponse.Result;
						var clearTextFileModel = FileModel.Cleartext(fileModel.Filename, clearText.Value, fileModel.SaveEncoding);
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

				if (!_visualCryptApiv2.TryDecodeVisualCryptText(fileModel.VisualCryptText).IsSuccess)
					throw new Exception("Aborting Save -  The data being saved is not valid VisualCrypt format.");

				byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(fileModel.VisualCryptText);
				File.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);
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

				var encryptResponse = _visualCryptApiv2.Encrypt2(new ClearText(textBufferContents), KeyStore.GetSHA256PW32(), GetV2LogRoundsSetting(), context.Progress, context.CancellationToken);
				if (encryptResponse.IsSuccess)
				{
					var encodeResponse = _visualCryptApiv2.EncodeToVisualCryptText(encryptResponse.Result);
					if (encodeResponse.IsSuccess)
					{
						VisualCryptText visualCryptText = encodeResponse.Result;
						byte[] visualCryptTextBytes = fileModel.SaveEncoding.GetBytes(visualCryptText.Value);
						File.WriteAllBytes(fileModel.Filename, visualCryptTextBytes);
						response.Result = visualCryptText.Value;
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
			return _visualCryptApiv2.GenerateRandomPassword();
		}


		public Response<string> SanitizePassword(string unsanitizedPassword)
		{
			var response = new Response<string>();
			try
			{
				var sanitizePasswordResponse = _visualCryptApiv2.SanitizePassword(unsanitizedPassword);
				if (sanitizePasswordResponse.IsSuccess)
				{
					response.Result = sanitizePasswordResponse.Result.Value;
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