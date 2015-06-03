using System;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Desktop.Shared.Files;

namespace VisualCrypt.Desktop.Shared.Services
{
	public interface IEncryptionService
	{
		/// <summary>
		/// Returns a clear text file or an encrypted file.
		/// </summary>
		Response<FileModel> OpenFile(string filename);

		Response<FileModel> EncryptForDisplay(FileModel fileModel, string textBufferContents);

		Response<FileModel> DecryptForDisplay(FileModel fileModel, string textBufferContents);

		Response SetPassword(byte[] utf16LEPassword);

		Response ClearPassword();

		Response SaveEncryptedFile(FileModel fileModel);

		Response<string> EncryptAndSaveFile(FileModel fileModel, string textBufferContents);
	}
}