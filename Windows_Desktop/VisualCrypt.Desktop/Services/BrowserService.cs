using System.Diagnostics;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Services
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
