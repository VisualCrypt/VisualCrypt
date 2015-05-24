using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Shell32;
using VisualCrypt.Desktop.Models.Fonts;
using VisualCrypt.Desktop.Models.Printing;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop.State
{
    public static class ModelState
    {
        // ReSharper disable InconsistentNaming
        const string Key_VisualCrypt = @"Software\VisualCrypt";     // root key
        const string Key_HasRunOnce = "HasRunOnce";                 // 0: has not run once, 1: has run once
        const string Key_NotepadSettings = "NotepadSettings";       // string key containing the serialized settings
        // ReSharper restore InconsistentNaming

        static NotepadSettings _notepadSettings;
        static Defaults _defaults;
        static Transient _transient;


        public static FontSettings FontSettings
        {
            get { return _notepadSettings.FontSettings; }
        }

        public static PageSettings PageSettings
        {
            get { return _notepadSettings.PageSettings; }
        }

        public static EditorState EditorState
        {
            get { return _notepadSettings.EditorState; }
        }

        public static Defaults Defaults
        {
            get { return _defaults ?? (_defaults = new Defaults()); }
        }

        public static Transient Transient
        {
            get { return _transient ?? (_transient = new Transient()); }
        }

        public static void Init()
        {
            if (!TryGetSettings(out _notepadSettings))
            {
                _notepadSettings = CreateTransientSettings();
                SaveSettings();
            }
        }

        public static void SaveSettings()
        {
            RegistryKey visualCryptKey = null;
            try
            {

                var serializedSettings = Serializer<NotepadSettings>.Serialize(_notepadSettings);
                visualCryptKey = GetOrCreateVisualCryptKey();
                visualCryptKey.SetValue(Key_NotepadSettings, serializedSettings);

            }
            catch (Exception e)
            {
                if (Application.Current.MainWindow != null)
                    new MessageBoxService(Application.Current.MainWindow).ShowError(MethodBase.GetCurrentMethod(), e);
                else
                {
                    new MessageBoxService().ShowError(MethodBase.GetCurrentMethod(), e);
                }
            }
            finally
            {
                if (visualCryptKey != null)
                    visualCryptKey.Dispose();
            }
        }

        /// <summary>
        /// Initializes the persistable settings with default values.
        /// </summary>
        static NotepadSettings CreateTransientSettings()
        {
            var settings = new NotepadSettings
            {
                FontSettings = new FontSettings
                {
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = FontSizeListItem.PointsToPixels(10),
                    FontStretch = FontStretches.Normal,
                    FontStyle = FontStyles.Normal,
                    FontWeight = FontWeights.Normal
                },
                PageSettings = new PageSettings { Margin = 72 },
                EditorState = new EditorState
                {
                    IsStatusBarChecked = true,
                    IsWordWrapChecked = true,
                    IsSpellCheckingChecked = false
                },
                
            };
            return settings;
        }

        static bool TryGetSettings(out NotepadSettings notepadSettings)
        {
            RegistryKey visualCryptKey = null;
            try
            {
                visualCryptKey = GetOrCreateVisualCryptKey();
                if (visualCryptKey == null)
                {
                    notepadSettings = null;
                    return false;
                }

                TryPinToTaskbarIfApplicable(visualCryptKey);

                notepadSettings = GetNotepadSettings(visualCryptKey);
               
                if (notepadSettings != null && notepadSettings.EditorState != null && notepadSettings.FontSettings != null && notepadSettings.PageSettings != null)
                    return true;
            }
            catch (Exception e)
            {
                if (Application.Current.MainWindow != null)
                    new MessageBoxService(Application.Current.MainWindow).ShowError(MethodBase.GetCurrentMethod(), e);
                else
                {
                    new MessageBoxService().ShowError(MethodBase.GetCurrentMethod(), e);
                }
                notepadSettings = null;
            }
            finally
            {
                if (visualCryptKey != null)
                    visualCryptKey.Dispose();
            }
            return false;
        }

        static RegistryKey GetOrCreateVisualCryptKey()
        {
            Registry.CurrentUser.CreateSubKey(Key_VisualCrypt, RegistryKeyPermissionCheck.ReadWriteSubTree);
            return Registry.CurrentUser.OpenSubKey(Key_VisualCrypt, true);
        }

        static NotepadSettings GetNotepadSettings(RegistryKey registryKey)
        {
            var settingsString = registryKey.GetValue(Key_NotepadSettings) as string;
            if (settingsString == null || string.IsNullOrWhiteSpace(settingsString))
                return null;

            return Serializer<NotepadSettings>.Deserialize(settingsString);
        }



        static void TryPinToTaskbarIfApplicable(RegistryKey registryKey)
        {
            try
            {
                var hasRunOnce = 0;
                var value = registryKey.GetValue(Key_HasRunOnce);

                if (value is int)
                    hasRunOnce = (int)value;

                if (hasRunOnce == 0)
                {
                    PinUnpinTaskBar(Assembly.GetEntryAssembly().Location, true);
                    registryKey.SetValue(Key_HasRunOnce, 1);
                }
            }
            catch (Exception e)
            {
                //if (Application.Current.MainWindow != null)
                //    new MessageBoxService(Application.Current.MainWindow).ShowError(MethodBase.GetCurrentMethod(), e);
                //else
                //{
                //    new MessageBoxService().ShowError(MethodBase.GetCurrentMethod(), e);
                //}
            }

        }

        static void PinUnpinTaskBar(string filePath, bool pin)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);

            // create the shell application object
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
                }
            }

        }
    }
}