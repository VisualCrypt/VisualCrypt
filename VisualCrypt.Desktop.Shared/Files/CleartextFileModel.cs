using System.Text;
using VisualCrypt.Desktop.Shared.App;

namespace VisualCrypt.Desktop.Shared.Files
{
	public class CleartextFileModel : FileModelBase
	{
		/// <summary>
		/// Creates an Empty ClearText filemodel.
		/// </summary>
		public CleartextFileModel()
		{
			IsDirty = false;
			Filename = Constants.UntitledDotVisualCrypt;
			Contents = string.Empty;
			SaveEncoding = Encoding.UTF8;
		}

		public CleartextFileModel(string importedString, Encoding encoding, string fileNameOnlyWithExtension)
		{
			IsDirty = false;
			Filename = fileNameOnlyWithExtension;
			Contents = importedString ?? string.Empty;
			SaveEncoding = encoding;
		}
	}
}