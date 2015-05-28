using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using VisualCrypt.Desktop.Shared;

namespace VisualCrypt.Desktop.ModuleEncryption
{

    [ModuleExport(typeof(ModuleEncryption), InitializationMode = InitializationMode.OnDemand)]
    public class ModuleEncryption : IModule
    {
        readonly IModuleTracker _moduleTracker;

        [ImportingConstructor]
        public ModuleEncryption(IModuleTracker moduleTracker)
        {
            if (moduleTracker == null)
            {
                throw new ArgumentNullException("moduleTracker");
            }

           _moduleTracker = moduleTracker;
           _moduleTracker.RecordModuleConstructed(ModuleNames.ModuleEncryption);
        }

        public void Initialize()
        {
            _moduleTracker.RecordModuleInitialized(ModuleNames.ModuleEncryption);
            TestOpenFile();
        }

        private void TestOpenFile()
        {
            throw new NotImplementedException();
        }
    }
}
