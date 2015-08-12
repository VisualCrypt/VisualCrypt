using System;
using VisualCrypt.Cryptography.Portable.Apps.Settings;
using VisualCrypt.Windows.Static;

namespace VisualCrypt.Windows.Services
{
    class SettingsManager : ISettingsManager
    {
        public string CurrentDirectoryName { get; set; }

        public CryptographySettings CryptographySettings { get; set; }

        public EditorSettings EditorSettings { get; set; }

        public object FontSettings { get; set; }


        public void LoadOrInitSettings()
        {
            EditorSettings = new EditorSettings();
            FontSettings = new FontSettings();
            CryptographySettings = new CryptographySettings { LogRounds = 11 };
        }
    }
}
