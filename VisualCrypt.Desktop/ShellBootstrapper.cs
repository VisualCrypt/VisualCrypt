using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop
{
	public class ShellBootstrapper : MefBootstrapper
	{
		readonly ReplayLogger _replayLogger = new ReplayLogger();

		public override void Run(bool runWithDefaultConfiguration)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			base.Run(runWithDefaultConfiguration);
			stopWatch.Stop();
			_replayLogger.Log("Loading completed after {0}ms.".FormatInvariant(stopWatch.ElapsedMilliseconds),Category.Info, Priority.Low);
			Container.GetExportedValue<ShellViewModel>().OpenFromCommandLineOrNew();
		}

		protected override DependencyObject CreateShell()
		{
			SettingsManager.LoadOrInitSettings();
			return Container.GetExportedValue<ShellWindow>();
		}

		protected override void InitializeShell()
		{
			base.InitializeShell();

			Application.Current.MainWindow = (Window) Shell;
			Application.Current.MainWindow.Show();
		}

		protected override void ConfigureAggregateCatalog()
		{
			base.ConfigureAggregateCatalog();

			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof (Constants).Assembly));
			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof (ModuleEditor.ModuleEditor).Assembly));
			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof (ModuleEncryption.ModuleEncryption).Assembly));
		}

		/// <summary>
		/// Configures the CompositionContainer.
		/// The base implementation registers all the types direct instantiated by the bootstrapper with the container.
		/// and sets the ServiceLocator provider singleton.
		/// </summary>
		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();

			// Because we created the ReplayLogger and it needs to be used immediately, 
			// we compose it to satisfy any imports it has.
			Container.ComposeExportedValue(_replayLogger);
		}

		

		/// <summary>
		/// Create the <see cref="ILoggerFacade"/> used by the bootstrapper.
		/// </summary>
		/// <remarks>
		/// The base implementation returns a new TextLogger.
		/// </remarks>
		/// <returns>
		/// A ReplayLogger.
		/// </returns>
		protected override ILoggerFacade CreateLogger()
		{
			// Because the ShellWindow is displayed after most of the interesting boostrapper work has been performed,
			// this quickstart uses a special logger class to hold on to early log entries and display them 
			// after the UI is visible.
			return _replayLogger;
		}
	}
}