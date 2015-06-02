using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using VisualCrypt.Desktop.Shared;

namespace VisualCrypt.Desktop.ModuleEncryption
{
	[ModuleExport(typeof (ModuleEncryption), InitializationMode = InitializationMode.OnDemand)]
	public class ModuleEncryption : IModule
	{
		readonly ILoggerFacade _logger;

		[ImportingConstructor]
		public ModuleEncryption(ILoggerFacade logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			_logger = logger;
			_logger.Log("{0} constructed.".FormatInvariant(GetType().Name),Category.Info, Priority.Low);
		}


		public void Initialize()
		{
			_logger.Log("{0} initialized.".FormatInvariant(GetType().Name), Category.Info, Priority.Low);
		}
	}
}