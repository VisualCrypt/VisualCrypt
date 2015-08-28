using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Shell32;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models.Settings;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Desktop.Settings;

namespace VisualCrypt.Desktop.Services
{
	
	public  class SettingsManager : ISettingsManager
	{
		readonly ILog _log;
		string _currentDirectoryName;
	
		public SettingsManager()
		{
			_log = Service.Get<ILog>();
			FactorySettings();
		}

		public  EditorSettings EditorSettings { get; private set; }

	    public  object FontSettings { get; private set; }

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

		public  void LoadOrInitSettings()
		{
		    Tuple<EditorSettings, FontSettings, CryptographySettings> settings = TryGetSettings();
			if (settings == null || settings.Item1 == null || settings.Item2 == null || settings.Item3 == null)
			{
				_log.Debug("Could not load settings, using factory settings.");
				FactorySettings();
				SaveSettings();
			}
			else
			{
				EditorSettings = settings.Item1;
				FontSettings = settings.Item2;
				CryptographySettings = settings.Item3;
				_log.Debug("Settings successfully loaded!.");
			}
				

		

			TryPinToTaskbarOnFirstRun();

            EditorSettings.PropertyChanged += SettingsChanged;
		    CryptographySettings.PropertyChanged += SettingsChanged;
            ((FontSettings)FontSettings).PropertyChanged += SettingsChanged;
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
				_log.Debug("Settings saved!");
			}
			catch (Exception e)
			{
				_log.Exception(e);
			}
		}


		 void  FactorySettings()
		{
			EditorSettings =  new EditorSettings
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
		    CryptographySettings = new CryptographySettings {LogRounds = 13};
		}

		 Tuple<EditorSettings,FontSettings,CryptographySettings> TryGetSettings()
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
                        return new Tuple<EditorSettings, FontSettings, CryptographySettings>(editorSettings, fontSettings,
					        cryptoSettings);
					}
				}
			}
			catch (Exception e)
			{
				_log.Exception(e);
				
			}
			return null;
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
				_log.Debug("Pinned to Taskbar");
			}
			catch (Exception e)
			{
				_log.Exception(e);
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