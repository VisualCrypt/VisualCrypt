using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Cryptography.Net.Tests
{

    [TestClass]
    public class Localization_Tests
    {
        static Dictionary<CultureInfo, string> Cultures = new Dictionary<CultureInfo, string>();
        ResourceWrapper resourceWrapper = new ResourceWrapper();

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
                Assert.AreEqual(entry.Value, Resources.Hello_World);
            }

        }

        [TestMethod]
        public void Can_Switch_Culture_And_Notify_Subscribers()
        {
            bool hasCultureChanged = false;
            resourceWrapper.Info.CultureChanged += (s, e) =>
            {
                hasCultureChanged = true;
            };

            List<string> changedProperties = new List<string>();
            resourceWrapper.Info.PropertyChanged += (s, e) =>
             {
                 if (!changedProperties.Contains(e.PropertyName))
                     changedProperties.Add(e.PropertyName);
             };

            resourceWrapper.Info.SwitchCulture("ru");

            Assert.IsTrue(hasCultureChanged);
            Assert.IsTrue(changedProperties.Count == 5);
            Assert.IsTrue(resourceWrapper.Info.IsEN == false);
            Assert.IsTrue(resourceWrapper.Info.IsDE == false);
            Assert.IsTrue(resourceWrapper.Info.IsFR == false);
            Assert.IsTrue(resourceWrapper.Info.IsIT == false);
            Assert.IsTrue(resourceWrapper.Info.IsRU == true);
        }

        void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

    }
}