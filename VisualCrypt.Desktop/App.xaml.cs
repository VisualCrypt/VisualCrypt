using System.Windows;

namespace VisualCrypt.Desktop
{
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var bootstrapper = new ShellBootstrapper();
			bootstrapper.Run(false);
		}
	}
}