using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VisualCrypt.Language.Tests
{
    [TestClass]
    public class BuildTool_Tests
    {
        string[] args = new string[] { "-generateresourcewrapper" , @"..\..\..\..\..\Common\ApplicationLogic\VisualCrypt.Language\Strings" };
        [TestMethod]
        public void TestMethod1()
        {
            var ret  = BuildTools.Program.Main(args);
            Assert.AreEqual(ret, 0);
        }
    }
}
