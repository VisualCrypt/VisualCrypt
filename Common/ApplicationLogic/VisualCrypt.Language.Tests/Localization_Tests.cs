using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Language.Tests
{

    [TestClass]
    public class Localization_Tests
    {
        static readonly Dictionary<CultureInfo, string> Cultures = new Dictionary<CultureInfo, string>();
        readonly ResourceWrapper _resourceWrapper = new ResourceWrapper();

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Cultures.Add(new CultureInfo("en-US"), "Hello World!");
            Cultures.Add(new CultureInfo("de-DE"), "Hallo Welt!");
            Cultures.Add(new CultureInfo("fr-FR"), "Bonjour tout le monde!");
            Cultures.Add(new CultureInfo("it-IT"), "Ciao, mondo!");
            Cultures.Add(new CultureInfo("ru-RU"), "Привет, мир!");
        }

        [TestMethod]
        public void Can_Access_Localized_Strings()
        {
            foreach (KeyValuePair<CultureInfo, string> entry in Cultures)
            {
                SetCulture(entry.Key);
                Assert.AreEqual(entry.Value, _resourceWrapper.termCancel);
            }

        }

        [TestMethod]
        public void Can_Switch_Culture_And_Notify_Subscribers()
        {
            bool hasCultureChanged = false;
            _resourceWrapper.Info.CultureChanged += (s, e) =>
            {
                hasCultureChanged = true;
            };

            List<string> changedProperties = new List<string>();
            _resourceWrapper.Info.PropertyChanged += (s, e) =>
             {
                 if (!changedProperties.Contains(e.PropertyName))
                     changedProperties.Add(e.PropertyName);
             };

            _resourceWrapper.Info.SwitchCulture("ru");

            Assert.IsTrue(hasCultureChanged);
            Assert.IsTrue(changedProperties.Count == 5);
            Assert.IsTrue(_resourceWrapper.Info.IsEN == false);
            Assert.IsTrue(_resourceWrapper.Info.IsDE == false);
            Assert.IsTrue(_resourceWrapper.Info.IsFR == false);
            Assert.IsTrue(_resourceWrapper.Info.IsIT == false);
            Assert.IsTrue(_resourceWrapper.Info.IsRU);
        }

        void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

    }
}