using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Desktop.Settings;
using System.Text;
using VisualCrypt.Applications.Services.PortableImplementations;

namespace VisualCrypt.Desktop.Services
{

    sealed class SettingsManager : AbstractSettingsManager
    {
       
        public override string CurrentDirectoryName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_currentDirectoryName) || !Directory.Exists(_currentDirectoryName))
                    _currentDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
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
                FontFamily = new FontFamily("Consolas"),
                FontSize = FontSizeListItem.PointsToPixels(11),
                FontStretch = FontStretches.Normal,
                FontStyle = FontStyles.Normal,
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
            var settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return File.ReadAllText(Path.Combine(settingsFolder, SettingsFilename), Encoding.Unicode);
        }

        protected override void WriteSettingsFile(string settingsFile)
        {
            var settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            File.WriteAllText(Path.Combine(settingsFolder, SettingsFilename), settingsFile, Encoding.Unicode);
        }
    }
}