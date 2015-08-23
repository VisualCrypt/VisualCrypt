using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Logging;
using Prism.Mef;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop.Tests
{
	[TestClass]
	public class TestSetup : MefBootstrapper
	{
		readonly ReplayLogger _replayLogger = new ReplayLogger();

		[AssemblyInitialize]
		public static void MEFSetup(TestContext context)
		{
			var bootstrapper = new TestSetup();
			bootstrapper.Run(true);
		}

		protected override System.Windows.DependencyObject CreateShell()
		{
		    var settings = new SettingsManager(new EmptyLogger());
            settings.LoadOrInitSettings();
			return Container.GetExportedValue<ShellWindow>();
		}

		protected override void ConfigureAggregateCatalog()
		{
			base.ConfigureAggregateCatalog();

			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellBootstrapper).Assembly));
			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Constants).Assembly));
			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ModuleEditor.ModuleEditor).Assembly));
			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ModuleEncryption.ModuleEncryption).Assembly));
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
	}
}
