using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using VisualCrypt.Desktop.Shared;

namespace VisualCrypt.Desktop.ModuleEditor
{
	[ModuleExport(typeof (ModuleEditor), DependsOnModuleNames = new string[] {})]
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
			_logger.Log("{0} constructed.".FormatInvariant(GetType().Name),Category.Info, Priority.Low);
		}


		public void Initialize()
		{
			_logger.Log("{0} initialized.".FormatInvariant(GetType().Name), Category.Info, Priority.Low);
		}
	}
}