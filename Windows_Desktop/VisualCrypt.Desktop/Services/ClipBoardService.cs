using System.Windows;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Services
{
	public class ClipBoardService : IClipBoardService
	{
		public void CopyText(string text)
		{
			Clipboard.SetText(text, TextDataFormat.Text);
		}
	}
}
