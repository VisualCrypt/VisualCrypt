using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Shell32;
using VisualCrypt.Cryptography.Portable.Settings;
using VisualCrypt.Desktop.Shared.Settings;

namespace VisualCrypt.Desktop.Shared.App
{
	[Export(typeof(ISettingsManager))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public  class SettingsManager
	{
		readonly ILoggerFacade Logger;
		string _currentDirectoryName;

		public  EditorSettings EditorSettings { get; private set; }

	    public  FontSettings FontSettings { get; private set; }

        public  CryptographySettings CryptographySettings { get; private set; }

        public  string CurrentDirectoryName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_currentDirectoryName) || !Directory.Exists(_currentDirectoryName))
					return Constants.DefaultDirectoryName;
				return _currentDirectoryName;
			}
			set { _currentDirectoryName = value; }
		}

		 SettingsManager()
		{
			try
			{
				Logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
			}
			catch (InvalidOperationException)
			{
				// Work around the "ServicelocationProvider must be set" error in VS Xaml designer
				// when editing ShellWindow.xaml
			}

			FactorySettings();
		}

		public  void LoadOrInitSettings()
		{
		    Tuple<EditorSettings, FontSettings, CryptographySettings> settings;
            if (!TryGetSettings(out settings))
			{
				Logger.Log("Could not load settings, using factory settings.", Category.Warn, Priority.Medium);
				FactorySettings();
				SaveSettings();
			}
			else
				Logger.Log("Settings successfully loaded!.", Category.Info, Priority.Medium);

			EditorSettings = settings.Item1;
		    FontSettings = settings.Item2;
		    CryptographySettings = settings.Item3;

			TryPinToTaskbarOnFirstRun();

            EditorSettings.PropertyChanged += SettingsChanged;
		    CryptographySettings.PropertyChanged += SettingsChanged;
            FontSettings.PropertyChanged += SettingsChanged;
        }

         void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveSettings();
        }

        public  void SaveSettings()
		{
			try
			{
				if (EditorSettings == null)
					return;
				var serializedSettings = Serializer<EditorSettings>.Serialize(EditorSettings);

				using (var visualCryptKey = GetOrCreateVisualCryptKey())
					visualCryptKey.SetValue(Constants.Key_EditorSettings, serializedSettings);
				Logger.Log("Settings saved!", Category.Info,
					Priority.Low);
			}
			catch (Exception e)
			{
				Logger.Log(e.Message, Category.Exception, Priority.Medium);
			}
		}


		 void  FactorySettings()
		{
			EditorSettings =  new EditorSettings
			{
				IsStatusBarChecked = true,
				IsWordWrapChecked = true,
				IsSpellCheckingChecked = false,
				PrintMargin = 72,
				
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
		    CryptographySettings = new CryptographySettings {LogRounds = 10};
		}

		 bool TryGetSettings(out Tuple<EditorSettings,FontSettings,CryptographySettings> settings)
		{
             
            try
			{
				using (RegistryKey visualCryptKey = GetOrCreateVisualCryptKey())
				{
					if (visualCryptKey != null)
					{
						var editorSettingsString = visualCryptKey.GetValue(Constants.Key_EditorSettings) as string;
                        var fontSettingsString = visualCryptKey.GetValue(Constants.Key_FontSettings) as string;
                        var cryptoSettingsString = visualCryptKey.GetValue(Constants.Key_CryptoSettings) as string;

					    EditorSettings editorSettings = null;
                        if (editorSettingsString != null && !string.IsNullOrWhiteSpace(editorSettingsString))
						{
							editorSettings = Serializer<EditorSettings>.Deserialize(editorSettingsString);
						}
                        FontSettings fontSettings = null;
                        if (fontSettingsString != null && !string.IsNullOrWhiteSpace(fontSettingsString))
                        {
                            fontSettings = Serializer<FontSettings>.Deserialize(fontSettingsString);
                        }
                        CryptographySettings cryptoSettings = null;
                        if (cryptoSettingsString != null && !string.IsNullOrWhiteSpace(cryptoSettingsString))
                        {
                            cryptoSettings = Serializer<CryptographySettings>.Deserialize(cryptoSettingsString);
                        }
                        settings = new Tuple<EditorSettings, FontSettings, CryptographySettings>(editorSettings, fontSettings,
					        cryptoSettings);
					    return true;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Log(e.Message, Category.Exception, Priority.Medium);
			}
			settings = null;
			return false;
		}

		 RegistryKey GetOrCreateVisualCryptKey()
		{
			Registry.CurrentUser.CreateSubKey(Constants.Key_VisualCrypt, RegistryKeyPermissionCheck.ReadWriteSubTree);
			return Registry.CurrentUser.OpenSubKey(Constants.Key_VisualCrypt, true);
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
						hasRunOnce = (int) value;

					if (hasRunOnce == 0)
					{
						PinUnpinTaskBar(Assembly.GetEntryAssembly().Location, true);
						visualCryptKey.SetValue(Constants.Key_HasRunOnce, 1);
					}
				}
				Logger.Log("Pinned to Taskbar", Category.Info, Priority.Low);
			}
			catch (Exception e)
			{
				Logger.Log(e.Message, Category.Exception, Priority.Low);
			}
		}

		 void PinUnpinTaskBar(string filePath, bool pin)
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