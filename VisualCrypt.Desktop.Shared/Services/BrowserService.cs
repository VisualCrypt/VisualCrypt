using System.Diagnostics;
using VisualCrypt.Cryptography.Portable;

namespace VisualCrypt.Desktop.Shared.Services
{
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
