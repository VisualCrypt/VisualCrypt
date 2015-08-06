using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Language
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

		public string constUntitledDotVisualCrypt
		{
			get
			{
				return new[] {  "Untitled.visualcrypt",  "Unbenannt.visualcrypt" }.GetLocalizedString();
			}
		}

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

		public string termEncrypt
		{
			get
			{
				return new[] { "Encrypt", "Verschlüsseln" }.GetLocalizedString();
			}
		}

		public string termDecrypt
		{
			get
			{
				return new[] { "Decrypt", "Entschlüsseln" }.GetLocalizedString();
			}
		}

		public string termPassword
		{
			get
			{
				return new[] { "Password", "Passwort" }.GetLocalizedString();
			}
		}

		public string termSetPassword
		{
			get
			{
				return new[] { "Set Password", "Passwort setzen" }.GetLocalizedString();
			}
		}

		public string termSave
		{
			get
			{
				return new[] { "Save", "Speichern" }.GetLocalizedString();
			}
		}

		public string termCopyToClipboard
		{
			get
			{
				return new[] { "Copy to Clipboard", "in die Zwischenablage" }.GetLocalizedString();
			}
		}

		public string termCancel
		{
			get
			{
				return new[] { "Cancel", "Abbrechen" }.GetLocalizedString();
			}
		}

		#region Status Bars

		public string plaintextStatusbarText
		{
			get
			{
				return new[] { "Plaintext | {0} | {1}", "Klartext | {0} | {1}" }.GetLocalizedString();
			}
		}

		public string plaintextStatusbarPositionInfo
		{
			get
			{
				return new[] { "Ln {0}, Col {1} | Pos {2}/{3}", "Zl {0}, Sp {1} | Pos {2}/{3}" }.GetLocalizedString();
			}
		}

		public string encrpytedStatusbarText
		{
			get
			{
				return new[] { "VisualCrypt {0}, AES 256 Bit, 2^{1} Rounds , {2} Ch.", "VisualCrypt {0}, AES 256 Bit, 2^{1}-fach, {2} Zeichen" }.GetLocalizedString();
			}
		}

		public string encProgr_EncryptingRandomKey
		{
			get
			{
				return new[] { "Encrypting Random Key..., ", "Random Key verschlüsseln..." }.GetLocalizedString();
			}
		}

		public string encProgr_EncryptingMessage
		{
			get
			{
				return new[] { "Encrypting Message..., ", "Text verschlüsseln..." }.GetLocalizedString();
			}
		}

		public string encProgr_EncryptingMAC
		{
			get
			{
				return new[] { "Encrypting MAC..., ", "MAC verschlüsseln..." }.GetLocalizedString();
			}
		}

		public string encProgr_DecryptingMAC
		{
			get
			{
				return new[] { "Decrypting MAC..., ", "MAC entschlüsseln..." }.GetLocalizedString();
			}
		}

		public string encProgr_DecryptingRandomKey
		{
			get
			{
				return new[] { "Decrypting Random Key..., ", "Random Key entschlüsseln..." }.GetLocalizedString();
			}
		}

		public string encProgr_DecryptingMessage
		{
			get
			{
				return new[] { "Decrypting Message...", "Text entschlüsseln..." }.GetLocalizedString();
			}
		}
		public string encProgr_CalculatingMAC
		{
			get
			{
				return new[] { "Calculating MAC...", "MAC berechnen..." }.GetLocalizedString();
			}
		}

		public string encProgr_ProcessingKey
		{
			get
			{
				return new[] { "Processing Key...", "Schlüssel verarbeiten..." }.GetLocalizedString();
			}
		}

		public string operationEncryption
		{
			get
			{
				return new[] { "Encrypting:", "Verschlüsseln:" }.GetLocalizedString();
			}
		}

		public string operationDecryption
		{
			get
			{
				return new[] { "Decrypting:", "Entschlüsseln:" }.GetLocalizedString();
			}
		}

		public string operationEncryptAndSave
		{
			get
			{
				return new[] { "Encrypting and saving:", "Verschlüsseln und speichern:" }.GetLocalizedString();
			}
		}

		public string operationDecryptOpenedFile
		{
			get
			{
				return new[] { "Decrypting the file being opened:", "Entschlüsseln der ladenden Datei:" }.GetLocalizedString();
			}
		}

		public string msgPasswordError
		{
			get
			{
				return new[] { "Incorrect password or corrupted/forged message.", "Falsches Passwort oder beschädigte/verfälschte Daten." }.GetLocalizedString();
			}
		}

		
		


		
		


		#endregion
















		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
