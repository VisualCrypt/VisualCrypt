using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;

namespace VisualCrypt.Windows.Static
{
    class SettingsManager
    {
        static SettingsManager()
        {
            EditorSettings = FactorySettings();
            CurrentDirectoryName = string.Empty;
        }

        static EditorSettings FactorySettings()
        {
            return new EditorSettings
            {
                IsStatusBarChecked = true,
                IsWordWrapChecked = true,
                IsSpellCheckingChecked = false,
                PrintMargin = 72,
                FontSettings = new FontSettings
                {
                    FontFamily = new FontFamily("Consolas"),
                    //FontSize = FontSizeListItem.PointsToPixels(11),
                    //FontStretch = FontStretches.Normal,
                    //FontStyle = FontStyles.Normal,
                    FontWeight = FontWeights.Normal
                },
                CryptographySettings = new CryptographySettings { LogRounds = 10 }
            };
        }
        public static string CurrentDirectoryName { get; internal set; }

        public static EditorSettings EditorSettings { get; private set; }

        public static void SaveSettings(object settingValueForLoggingOnly, [CallerMemberName] string callerMemberName = null)
        {
            try
            {
                if (EditorSettings == null)
                    return;
                var serializedSettings = Serializer<EditorSettings>.Serialize(EditorSettings);

                //using (var visualCryptKey = GetOrCreateVisualCryptKey())
                //    visualCryptKey.SetValue(Constants.Key_NotepadSettings, serializedSettings);
                //Logger.Log("Setting '{0}:{1}' saved!".FormatInvariant(callerMemberName, settingValueForLoggingOnly), Category.Info,
                //    Priority.Low);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
