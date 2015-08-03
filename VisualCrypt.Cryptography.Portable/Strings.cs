using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using VisualCrypt.Cryptography.Portable.Annotations;

namespace VisualCrypt.Cryptography.Portable
{
	public class Strings : INotifyPropertyChanged
	{
		#region Localization engine

		readonly List<string> _propertyNames;

		public Strings()
		{
			Localization.SetLanguageOrder(new[] { "EN", "DE" });
			Localization.SetCurrentLocale("EN");

			_propertyNames = new List<string>();
			var properties = GetType().GetRuntimeProperties();
			foreach (var propertyInfo in properties)
			{
				_propertyNames.Add(propertyInfo.Name);
			}
		}

		internal void SwitchLocale(string locale)
		{
			Localization.SetCurrentLocale(locale);
			foreach (var prop in _propertyNames)
			{
				OnPropertyChanged(prop);
			}
		}

		public bool IsEN
		{
			get { return Localization.GetCurrentLocale() == "EN"; }
		}

		public bool IsDE
		{
			get { return Localization.GetCurrentLocale() == "DE"; }
		}

		#endregion

		#region Languages

		// MenuItems


		public string miFile
		{
			get
			{
				return new[] { "_File", "_Datei" }.GetLocalizedString();
			}
		}

		public string miFileNew
		{
			get
			{
				return new[] { "_New", "_Neu" }.GetLocalizedString();
			}
		}

		public string miFileOpen
		{
			get
			{
				return new[] { "_Open...", "_Öffnen..." }.GetLocalizedString();
			}
		}

		public string miFileSave
		{
			get
			{
				return new[] { "_Save", "_Speichern" }.GetLocalizedString();
			}
		}

		public string miFileSaveAs
		{
			get
			{
				return new[] { "Save _As..", "Speichern _unter..." }.GetLocalizedString();
			}
		}

		public string miFileExportClearText
		{
			get
			{
				return new[] { "_Export Cleartext", "Klartext _exportieren..." }.GetLocalizedString();
			}
		}
		public string miFileImportWithEnconding
		{
			get
			{
				return new[] { "_Import With Encoding...", "Importieren mit Encoding..." }.GetLocalizedString();
			}
		}
		public string miFilePrint
		{
			get
			{
				return new[] { "_Print...", "_Drucken..." }.GetLocalizedString();
			}
		}
		public string miFileExit
		{
			get
			{
				return new[] { "E_xit", "_Beenden" }.GetLocalizedString();
			}
		}

		// Menu VisualCrypt

		public string miVC
		{
			get
			{
				return new[] { "_VisualCrypt", "_VisualCrypt" }.GetLocalizedString();
			}
		}

		public string miVCSetPassword
		{
			get
			{
				return new[] { "Set _Password...", "_Passwort setzen..." }.GetLocalizedString();
			}
		}

		public string miVCChangePassword
		{
			get
			{
				return new[] { "Change _Password...", "_Passwort ändern..." }.GetLocalizedString();
			}
		}

		public string miVCEncrypt
		{
			get
			{
				return new[] { "_Encrypt", "_Verschlüsseln" }.GetLocalizedString();
			}
		}

		public string miVCDecrypt
		{
			get
			{
				return new[] { "_Decrpyt", "_Entschlüsseln" }.GetLocalizedString();
			}
		}

		public string miVCSettings
		{
			get
			{
				return new[] { "_Settings...", "E_instellungen..." }.GetLocalizedString();
			}
		}











		public string miLanguage
		{
			get
			{
				return new[] { "_Language", "_Sprache" }.GetLocalizedString();
			}
		}



		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
