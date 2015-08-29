using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Desktop.Settings;
using System.Text;

namespace VisualCrypt.Desktop.Services
{

    public class SettingsManager : ISettingsManager
    {
        const string SettingsFilename = "VisualCryptSettings.txt";
        readonly ILog _log;
        string _currentDirectoryName;

        public SettingsManager()
        {
            _log = Service.Get<ILog>();

            // The settings must be initialized early so that DataBinding gets references
            // to the correct instances of EditorSettings, FontSettings and CryptographySettings.
            var success = LoadSettings();
            if (!success)
                FactorySettings();

            EditorSettings.PropertyChanged += (s, e) => SaveSettings();
            CryptographySettings.PropertyChanged += (s, e) => SaveSettings();
            FontSettings.PropertyChanged += (s, e) => SaveSettings();
        }

        public EditorSettings EditorSettings { get; private set; }

        public FontSettings FontSettings { get; private set; }

        public CryptographySettings CryptographySettings { get; private set; }

        public string CurrentDirectoryName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_currentDirectoryName) || !Directory.Exists(_currentDirectoryName))
                    _currentDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                return _currentDirectoryName;
            }
            set { _currentDirectoryName = value; }
        }

      

        void SaveSettings()
        {
            try
            {
                if (EditorSettings == null)
                    return;

                // Collect the Settings
                var settings = new Models.SerializableVisualCryptSettings
                {
                    EditorSettings = EditorSettings,
                    CryptographySettings = CryptographySettings,
                    FontSettings = FontSettings
                };
                // Serialize
                var serializedSettings = Serializer<Models.SerializableVisualCryptSettings>.Serialize(settings);

                // Save
                var success = TrySaveSettingsFile(serializedSettings);
                if (!success)
                    _log.Debug("Could not save settings!");
                else
                    _log.Debug("Settings saved!");
            }
            catch (Exception e)
            {
                _log.Debug($"An exception occured in SaveSettings: {e.Message}");
            }
        }

        bool LoadSettings()
        {
            try
            {
                // Load
                string serializedSettings = GetSettingsFileOrNull();
                if (serializedSettings == null)
                {
                    _log.Debug("Could not load settings!");
                    return false;
                }
                // Deserialize
                var settings = Serializer<Models.SerializableVisualCryptSettings>.Deserialize(serializedSettings);

                // Distribute
                EditorSettings = settings.EditorSettings;
                CryptographySettings = settings.CryptographySettings;
                FontSettings = settings.FontSettings;
                _log.Debug("Settings loaded.");
                return true;

            }
            catch (Exception e)
            {
                _log.Debug($"An exception occured in LoadSettings: {e.Message}");
                return false;
            }
        }

        void FactorySettings()
        {
            _log.Debug("Applying factory settings.");

            EditorSettings = new EditorSettings
            {
                IsStatusBarVisible = true,
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
        }

        string GetSettingsFileOrNull()
        {
            string settings = null;
            try
            {
                var settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                settings = File.ReadAllText(Path.Combine(settingsFolder, SettingsFilename), Encoding.Unicode);
                return settings;
            }
            catch (Exception e)
            {
                _log.Exception(e);
            }
            return settings;
        }

        bool TrySaveSettingsFile(string settingsFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(settingsFile))
                    throw new ArgumentException("The setting to save are null or white space", "settingsFile");

                var settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                File.WriteAllText(Path.Combine(settingsFolder, SettingsFilename), settingsFile, Encoding.Unicode);
                return true;
            }
            catch (Exception e)
            {
                _log.Exception(e);
                return false;
            }
        }




    }
}