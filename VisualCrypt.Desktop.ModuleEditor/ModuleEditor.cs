using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using VisualCrypt.Desktop.Shared;

namespace VisualCrypt.Desktop.ModuleEditor
{

    [ModuleExport(typeof(ModuleEditor), DependsOnModuleNames = new string[] { })]
    public class ModuleEditor : IModule
    {
        private readonly ILoggerFacade _logger;
        private readonly IModuleTracker _moduleTracker;


        [ImportingConstructor]
        public ModuleEditor(ILoggerFacade logger, IModuleTracker moduleTracker)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (moduleTracker == null)
            {
                throw new ArgumentNullException("moduleTracker");
            }

            _logger = logger;
            _moduleTracker = moduleTracker;
            _moduleTracker.RecordModuleConstructed(ModuleNames.ModuleEditor);
        }


        public void Initialize()
        {
            _logger.Log("ModuleEditor Initialize().", Category.Info, Priority.Medium);
            _moduleTracker.RecordModuleInitialized(ModuleNames.ModuleEditor);
        }
    }
}
