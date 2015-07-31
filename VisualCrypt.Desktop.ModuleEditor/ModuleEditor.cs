using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using VisualCrypt.Desktop.Shared.PrismSupport;

namespace VisualCrypt.Desktop.ModuleEditor
{
	[ModuleExport(typeof(ModuleEditor), InitializationMode = InitializationMode.WhenAvailable, DependsOnModuleNames = new[] { ModuleNames.ModuleEncryption })]
	public class ModuleEditor : IModule
	{
		readonly ILoggerFacade _logger;

		[ImportingConstructor]
		public ModuleEditor(ILoggerFacade logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			_logger = logger;
			_logger.Log(string.Format("{0} constructed.", GetType().Name), Category.Info, Priority.Low);
		}


		public void Initialize()
		{
			_logger.Log(string.Format("{0} initialized.", GetType().Name), Category.Info, Priority.Low);
		}
	}
}