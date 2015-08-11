using System.Windows;
using VisualCrypt.Cryptography.Portable;

namespace VisualCrypt.Desktop.Shared.Services
{
	public class ClipBoardService : IClipBoardService
	{
		public void CopyText(string text)
		{
			Clipboard.SetText(text, TextDataFormat.Text);
		}
	}
}
