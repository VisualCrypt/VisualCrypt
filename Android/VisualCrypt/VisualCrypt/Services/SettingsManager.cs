using System;
using System.IO;
using System.Text;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Droid.Models;

namespace VisualCrypt.Droid.Services
{
    class SettingsManager : AbstractSettingsManager
    {

        public override string CurrentDirectoryName
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
            }
            set { }
        }

        protected override void FactorySettings()
        {
            _log.Debug("Applying factory settings.");

            EditorSettings = new EditorSettings
            {
                IsWordWrapChecked = true,
                IsSpellCheckingChecked = false,
                PagePadding = 72,
                IsToolAreaVisible = false


            };
            FontSettings = new FontSettings
            {
                //FontFamily = new FontFamily("Lucida Console"),
                //FontSize = 11,
                //FontStretch = FontStretch.Normal,
                //FontStyle = FontStyle.Normal,
                //FontWeight = FontWeights.Normal
            }
            ;
            CryptographySettings = new CryptographySettings { LogRounds = 13 };
            UpdateSettings = new UpdateSettings
            {
                Version = _aip.AssemblyVersion,
                SKU = _aip.AssemblyProduct,
                Date = DateTime.UtcNow,
                Notify = true
            };
        }

        protected override string ReadSettingsFile()
        {
            return File.ReadAllText(Path.Combine(CurrentDirectoryName, SettingsFilename), Encoding.Unicode);
        }

        protected override void WriteSettingsFile(string settingsFile)
        {
            File.WriteAllText(Path.Combine(CurrentDirectoryName, SettingsFilename), settingsFile, Encoding.Unicode);
        }
    }
}