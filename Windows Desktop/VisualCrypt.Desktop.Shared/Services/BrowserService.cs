using System.ComponentModel.Composition;
using System.Diagnostics;
using VisualCrypt.Applications.Apps.Services;

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
