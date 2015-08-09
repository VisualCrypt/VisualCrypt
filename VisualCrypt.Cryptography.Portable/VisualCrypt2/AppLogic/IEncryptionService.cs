using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic
{
	public interface IEncryptionService
	{
		/// <summary>
		/// Returns a clear text file or an encrypted file.
		/// </summary>
		Response<FileModel> OpenFile(string filename);

		Response<FileModel> EncryptForDisplay(FileModel fileModel, string textBufferContents, RoundsExponent roundsExponent,
            LongRunningOperationContext context);

		Response<FileModel> DecryptForDisplay(FileModel fileModel, string textBufferContents,
			LongRunningOperationContext context);

		Response SetPassword(string unprunedUTF16LEPassword);

		Response ClearPassword();

		Response SaveEncryptedFile(FileModel fileModel);

		Response<string> EncryptAndSaveFile(FileModel fileModel, string textBufferContents, RoundsExponent roundsExponent,
            LongRunningOperationContext context);

		Response<string> GenerateRandomPassword();

		Response<string> SanitizePassword(string unsanitizedPassword);
	}
}