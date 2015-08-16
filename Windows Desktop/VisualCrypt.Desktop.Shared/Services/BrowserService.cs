using System.ComponentModel.Composition;
using System.Diagnostics;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.Apps.Services;

namespace VisualCrypt.Desktop.Shared.Services
{
	[Export(typeof(IBrowserService))]
	public class BrowserService : IBrowserService
	{
		public void LaunchUrl(string url)
		{
			using (
					var process = new Process { StartInfo = { UseShellExecute = true, FileName = url} })
					process.Start();
		}
	}
}
