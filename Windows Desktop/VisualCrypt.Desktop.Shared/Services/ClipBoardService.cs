using System.ComponentModel.Composition;
using System.Windows;
using VisualCrypt.Applications.Apps.Services;

namespace VisualCrypt.Desktop.Shared.Services
{
	[Export(typeof(IClipBoardService))]
	public class ClipBoardService : IClipBoardService
	{
		public void CopyText(string text)
		{
			Clipboard.SetText(text, TextDataFormat.Text);
		}
	}
}
