using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Shell32;
using VisualCrypt.Desktop.Shared.Settings;

namespace VisualCrypt.Desktop.Shared.App
{
	public static class SettingsManager
	{
		static readonly ILoggerFacade Logger;
		static string _currentDirectoryName;

		public static EditorSettings EditorSettings { get; private set; }

		public static string CurrentDirectoryName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_currentDirectoryName) || !Directory.Exists(_currentDirectoryName))
					return Constants.DefaultDirectoryName;
				return _currentDirectoryName;
			}
			set { _currentDirectoryName = value; }
		}

		static SettingsManager()
		{
			Logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
			Debug.Assert(Logger != null);

			EditorSettings = FactorySettings();
		}

		public static void LoadOrInitSettings()
		{
			EditorSettings settings;
			if (!TryGetSettings(out settings))
			{
				Logger.Log("Could not load settings, using factory settings.", Category.Warn, Priority.Medium);
				settings = FactorySettings();
				SaveSettings(settings);
			}
			else
				Logger.Log("Settings successfully loaded!.", Category.Info, Priority.Medium);

			EditorSettings = settings;

			TryPinToTaskbarOnFirstRun();
		}
	


		public static void SaveSettings(object settingValueForLoggingOnly, [CallerMemberName] string callerMemberName = null)
		{
			try
			{
				if (EditorSettings == null)
					return;
				var serializedSettings = Serializer<EditorSettings>.Serialize(EditorSettings);

				using (var visualCryptKey = GetOrCreateVisualCryptKey())
					visualCryptKey.SetValue(Constants.Key_NotepadSettings, serializedSettings);
				Logger.Log("Setting '{0}:{1}' saved!".FormatInvariant(callerMemberName,settingValueForLoggingOnly), Category.Info, Priority.Low);
			}
			catch (Exception e)
			{
				Logger.Log(e.Message, Category.Exception, Priority.Medium);
			}
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
					FontSize = FontSizeListItem.PointsToPixels(11),
					FontStretch = FontStretches.Normal,
					FontStyle = FontStyles.Normal,
					FontWeight = FontWeights.Normal
				},
				CryptographySettings = new CryptographySettings {  LogRounds = 10}
			};
		}

		static bool TryGetSettings(out EditorSettings editorSettings)
		{
			try
			{
				using (RegistryKey visualCryptKey = GetOrCreateVisualCryptKey())
				{
					if (visualCryptKey != null)
					{
						var settingsString = visualCryptKey.GetValue(Constants.Key_NotepadSettings) as string;
						if (settingsString != null && !string.IsNullOrWhiteSpace(settingsString))
						{
							editorSettings = Serializer<EditorSettings>.Deserialize(settingsString);
							if (editorSettings != null && editorSettings.FontSettings != null)
								return true;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Log(e.Message, Category.Exception, Priority.Medium);
			}
			editorSettings = null;
			return false;
		}

		static RegistryKey GetOrCreateVisualCryptKey()
		{
			Registry.CurrentUser.CreateSubKey(Constants.Key_VisualCrypt, RegistryKeyPermissionCheck.ReadWriteSubTree);
			return Registry.CurrentUser.OpenSubKey(Constants.Key_VisualCrypt, true);
		}


		static void TryPinToTaskbarOnFirstRun()
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