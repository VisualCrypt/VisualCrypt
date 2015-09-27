using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Desktop.Settings;
using System.Text;
using Microsoft.Win32;
using Shell32;
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
            TryPinToTaskbarOnFirstRun();
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

        void TryPinToTaskbarOnFirstRun()
        {
            try
            {
                using (var visualCryptKey = GetOrCreateVisualCryptKey())
                {
                    var hasRunOnce = 0;
                    var value = visualCryptKey.GetValue(Constants.Key_HasRunOnce);

                    if (value is int)
                        hasRunOnce = (int)value;

                    if (hasRunOnce == 0)
                    {
                        var success = PinUnpinTaskBar(Assembly.GetEntryAssembly().Location, true);
                        if (success)
                        {
                            _log.Debug("Pinned to Taskbar");
                            visualCryptKey.SetValue(Constants.Key_HasRunOnce, 1);
                        }
                           
                    }
                }
              
            }
            catch (Exception e)
            {
                _log.Exception(e);
            }
        }

        RegistryKey GetOrCreateVisualCryptKey()
        {
            Registry.CurrentUser.CreateSubKey(Constants.Key_VisualCrypt, RegistryKeyPermissionCheck.ReadWriteSubTree);
            return Registry.CurrentUser.OpenSubKey(Constants.Key_VisualCrypt, true);
        }

        bool PinUnpinTaskBar(string filePath, bool pin)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);

            var shellApplication = new Shell();

            string path = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            Folder directory = shellApplication.NameSpace(path);
            FolderItem link = directory.ParseName(fileName);

            FolderItemVerbs verbs = link.Verbs();
            for (int i = 0; i < verbs.Count; i++)
            {
                FolderItemVerb verb = verbs.Item(i);
             
                string verbName = verb.Name.Replace(@"&", string.Empty).ToLower();

                if ((pin && verbName.Equals("pin to taskbar")) || (!pin && verbName.Equals("unpin from taskbar")))
                {
                    verb.DoIt();
                    return true;
                }
            }
            return false;
        }
    }
}