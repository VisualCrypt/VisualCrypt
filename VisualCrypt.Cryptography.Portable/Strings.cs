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
		// Menu File

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
				return new[] { "Save _As...", "Speichern _unter..." }.GetLocalizedString();
			}
		}

		public string miFileExportClearText
		{
			get
			{
				return new[] { "_Export Cleartext...", "Klartext _exportieren..." }.GetLocalizedString();
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

		public string miEdit
		{
			get
			{
				return new[] { "_Edit", "_Bearbeiten" }.GetLocalizedString();
			}
		}

		public string miEditUndo
		{
			get
			{
				return new[] { "_Undo", "_Rückgängig" }.GetLocalizedString();
			}
		}

		public string miEditRedo
		{
			get
			{
				return new[] { "_Redo", "_Wiederherstellen" }.GetLocalizedString();
			}
		}

		public string miEditCut
		{
			get
			{
				return new[] { "Cu_t", "_Ausschneiden" }.GetLocalizedString();
			}
		}

		public string miEditCopy
		{
			get
			{
				return new[] { "_Copy", "_Kopieren" }.GetLocalizedString();
			}
		}

		public string miEditPaste
		{
			get
			{
				return new[] { "_Paste", "_Einfügen" }.GetLocalizedString();
			}
		}

		public string miEditFind
		{
			get
			{
				return new[] { "_Find...", "_Suchen..." }.GetLocalizedString();
			}
		}

		public string miEditFindNext
		{
			get
			{
				return new[] { "Find _Next", "_Nächste Fundstelle" }.GetLocalizedString();
			}
		}

		public string miEditFindPrevious
		{
			get
			{
				return new[] { "Find _Previous", "Vori_ge Fundstelle" }.GetLocalizedString();
			}
		}

		public string miEditReplace
		{
			get
			{
				return new[] { "_Replace...", "E_rsetzen..." }.GetLocalizedString();
			}
		}

		public string miEditDeleteLine
		{
			get
			{
				return new[] { "_Delete Line", "Zeile lösche_n" }.GetLocalizedString();
			}
		}

		public string miEditGoTo
		{
			get
			{
				return new[] { "_Go To...", "Ge_he zu Zeile..." }.GetLocalizedString();
			}
		}

		public string miEditSelectAll
		{
			get
			{
				return new[] { "Select _All", "_Alles markieren" }.GetLocalizedString();
			}
		}

		public string miEditInsertDateTime
		{
			get
			{
				return new[] { "_Insert Date, Time", "Datum, Zeit e_infügen" }.GetLocalizedString();
			}
		}

		public string miFormat
		{
			get
			{
				return new[] { "F_ormat", "F_ormat" }.GetLocalizedString();
			}
		}

		public string miFormatWordWrap
		{
			get
			{
				return new[] { "_Word Wrap", "_Zeilenumbruch" }.GetLocalizedString();
			}
		}

		public string miFormatCheckSpelling
		{
			get
			{
				return new[] { "Spe_llchecking", "Rechtschreib_prüfung" }.GetLocalizedString();
			}
		}

		public string miFormatFont
		{
			get
			{
				return new[] { "_Font...", "_Schriftart..." }.GetLocalizedString();
			}
		}

		public string miView
		{
			get
			{
				return new[] { "_View", "_Ansicht" }.GetLocalizedString();
			}
		}

		public string miViewStatusBar
		{
			get
			{
				return new[] { "_Status Bar", "_Statuszeile" }.GetLocalizedString();
			}
		}

		public string miViewToolArea
		{
			get
			{
				return new[] { "_Tool Area", "_Werkzeugbereich" }.GetLocalizedString();
			}
		}

		public string miViewLanguage
		{
			get
			{
				return new[] { "_Language", "S_prache" }.GetLocalizedString();
			}
		}

		public string miViewZoomLevelText // e.g. (Zoom 134%)
		{
			get
			{
				return new[] { "Zoom ({0}%)", "_Vergrößerung ({0}%)" }.GetLocalizedString();
			}
		}

		public string miViewZoomIn
		{
			get
			{
				return new[] { "Zoom _In", "Vergrö_ßern" }.GetLocalizedString();
			}
		}

		public string miViewZoomOut
		{
			get
			{
				return new[] { "Zoom _Out", "Ver_kleinern" }.GetLocalizedString();
			}
		}

		public string miHelp
		{
			get
			{
				return new[] { "_Help", "_Hilfe" }.GetLocalizedString();
			}
		}

		public string miHelpViewOnline
		{
			get
			{
				return new[] { "_View Help Online", "Hilfe (_Web)" }.GetLocalizedString();
			}
		}

		public string miHelpLog
		{
			get
			{
				return new[] { "_Log...", "_Log..." }.GetLocalizedString();
			}
		}

		public string miHelpAbout
		{
			get
			{
				return new[] { "_About VisualCrypt...", "_Über VisualCrpyt..." }.GetLocalizedString();
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
