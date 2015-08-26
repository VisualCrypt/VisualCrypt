using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Tests
{
    [TestClass]
    public class Init
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Bootstrapper.ConfigureFactory();
            Service.Register<IMessageBoxService, FakeOkMessageBoxService>(true, null, true);
        }

    }
}
