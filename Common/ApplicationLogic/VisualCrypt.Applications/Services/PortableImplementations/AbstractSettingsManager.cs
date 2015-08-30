using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Applications.Services.PortableImplementations
{
    public abstract class AbstractSettingsManager
    {
        protected const string SettingsFilename = "VisualCryptSettings.txt";
        protected readonly ILog _log;
        protected string _currentDirectoryName;

        public AbstractSettingsManager()
        {
            _log = Service.Get<ILog>();
            InitForBinding();
        }

        public EditorSettings EditorSettings { get; protected set; }

        public IFontSettings FontSettings { get; protected set; }

        public CryptographySettings CryptographySettings { get; protected set; }

        public abstract string CurrentDirectoryName { get; set; }

        protected abstract void FactorySettings();

        protected abstract string ReadSettingsFile();

        protected abstract void WriteSettingsFile(string settingsFile);

        void InitForBinding()
        {
            var success = LoadSettings();
            if (!success)
            { 
                FactorySettings();
                SaveSettings();
            }

            EditorSettings.PropertyChanged += (s, e) => SaveSettings();
            CryptographySettings.PropertyChanged += (s, e) => SaveSettings();
            FontSettings.PropertyChanged += (s, e) => SaveSettings();
        }

        void SaveSettings()
        {
            try
            {
                if (EditorSettings == null)
                    return;

                // Collect the Settings
                var settings = new VisualCryptSettings
                {
                    EditorSettings = EditorSettings,
                    CryptographySettings = CryptographySettings,
                    FontSettings = FontSettings
                };

                // Serialize, save
                var serializedSettings = Serialize(settings);
                WriteSettingsFile(serializedSettings);

                _log.Debug("Settings saved!");
            }
            catch (Exception e)
            {
                _log.Debug($"Could not save settings: {e.Message}");
            }
        }

        bool LoadSettings()
        {
            try
            {
                // Load, deserialize
                string serializedSettings = ReadSettingsFile();
                var settings = Deserialize(serializedSettings);

                // Distribute
                EditorSettings = settings.EditorSettings;
                CryptographySettings = settings.CryptographySettings;
                FontSettings = settings.FontSettings;
                _log.Debug("Settings loaded.");
                return true;
            }
            catch (Exception e)
            {
                _log.Debug($"Could not load settings: {e.Message}");
                return false;
            }
        }

        string Serialize(VisualCryptSettings settings)
        {
            using (var stream = new MemoryStream())
            {
                var ser = new DataContractSerializer(typeof(VisualCryptSettings), new[] { FontSettings.GetType() });
                ser.WriteObject(stream, settings);
                var data = stream.ToArray();
                var serialized = Encoding.UTF8.GetString(data, 0, data.Length);
                return serialized;
            }
        }

        VisualCryptSettings Deserialize(string data)
        {
            Type platformFontSettings = Service.Get<IFontSettings>().GetType();

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var ser = new DataContractSerializer(typeof(VisualCryptSettings), new[] { platformFontSettings });
                var settings = (VisualCryptSettings)ser.ReadObject(stream);
                return settings;
            }
        }
    }
}





