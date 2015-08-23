using System;
using System.ComponentModel.Composition;
using Prism.Logging;
using Prism.Mef.Modularity;
using Prism.Modularity;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Desktop.ModuleEncryption
{
	[ModuleExport(typeof (ModuleEncryption), InitializationMode = InitializationMode.WhenAvailable)]
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
			_logger.Log("{0} constructed.".FormatInvariant(GetType().Name), Category.Info, Priority.Low);
		}


		public void Initialize()
		{
			_logger.Log("{0} initialized.".FormatInvariant(GetType().Name), Category.Info, Priority.Low);
		}
	}
}