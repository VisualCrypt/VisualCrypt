using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Shell32;

namespace VisualCrypt.Desktop.Settings
{
	internal class OnFirstRunTasks
	{
		const string HasRunOnce = "HasRunOnce";
		// 0: The app has not yet been run | 1: It has been run at least one time

		public static void OnFirstRun()
		{
			try
			{
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\VisualCrypt", true))
				{
					if (registryKey == null) // The key is created by the installer.
						return;
					var hasRunOnce = (int) registryKey.GetValue(HasRunOnce);
					if (hasRunOnce == 0)
					{
						PinUnpinTaskBar(Assembly.GetEntryAssembly().Location, true);
						registryKey.SetValue(HasRunOnce, 1);
					}
				}
			}
			catch (Exception)
			{
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

			shellApplication = null;
		}
	}
}