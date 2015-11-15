using System;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.UWP.Models;

namespace VisualCrypt.UWP.Services
{
    sealed class SettingsManager : AbstractSettingsManager
    {

        public override string CurrentDirectoryName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_currentDirectoryName) || !Directory.Exists(_currentDirectoryName))
                    _currentDirectoryName = ApplicationData.Current.LocalFolder.Path;
                return _currentDirectoryName;
            }
            set { _currentDirectoryName = value; }
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
                FontFamily = new FontFamily("Lucida Console"),
                FontSize = 11,
                FontStretch = FontStretch.Normal,
                FontStyle = FontStyle.Normal,
                FontWeight = FontWeights.Normal
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
            return File.ReadAllText(Path.Combine(ApplicationData.Current.LocalFolder.Path, SettingsFilename), Encoding.Unicode);
        }

        protected override void WriteSettingsFile(string settingsFile)
        {
            File.WriteAllText(Path.Combine(ApplicationData.Current.LocalFolder.Path, SettingsFilename), settingsFile, Encoding.Unicode);
        }
    }
}
