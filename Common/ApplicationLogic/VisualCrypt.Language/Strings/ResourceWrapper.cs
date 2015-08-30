using System.ComponentModel;

namespace VisualCrypt.Language.Strings
{
    /// <summary>
    /// How to update:
    /// 0. Copy/paste all properties from Resources.Designer.cs
    /// 1. Replace 'static ' with ''
    /// 2. Replace 'return ResourceManager.GetString("' with 'return Resources.'
    /// 3. Replace '", resourceCulture);' with ';'
    /// TODO: write a tool for this.
    /// </summary>
    public class ResourceWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Info Info { get { return _info; } }
        readonly Info _info;
        public ResourceWrapper()
        {
            _info = new Info();
            Info.CultureChanged += (s, e) =>
            {
                var p = PropertyChanged;
                if (p != null)
                    p(this, new PropertyChangedEventArgs(null));
            };
        }

        #region copiedProperties

        /// <summary>
        ///   Looks up a localized string similar to Untitled.visualcrypt.
        /// </summary>
        public string constUntitledDotVisualCrypt
        {
            get
            {
                return Resources.constUntitledDotVisualCrypt;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Calculating MAC....
        /// </summary>
        public string encProgr_CalculatingMAC
        {
            get
            {
                return Resources.encProgr_CalculatingMAC;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypting MAC....
        /// </summary>
        public string encProgr_DecryptingMAC
        {
            get
            {
                return Resources.encProgr_DecryptingMAC;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypting Message....
        /// </summary>
        public string encProgr_DecryptingMessage
        {
            get
            {
                return Resources.encProgr_DecryptingMessage;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypting Random Key....
        /// </summary>
        public string encProgr_DecryptingRandomKey
        {
            get
            {
                return Resources.encProgr_DecryptingRandomKey;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypting MAC....
        /// </summary>
        public string encProgr_EncryptingMAC
        {
            get
            {
                return Resources.encProgr_EncryptingMAC;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypting Message....
        /// </summary>
        public string encProgr_EncryptingMessage
        {
            get
            {
                return Resources.encProgr_EncryptingMessage;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypting Random Key....
        /// </summary>
        public string encProgr_EncryptingRandomKey
        {
            get
            {
                return Resources.encProgr_EncryptingRandomKey;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Processing Key....
        /// </summary>
        public string encProgr_ProcessingKey
        {
            get
            {
                return Resources.encProgr_ProcessingKey;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to VisualCrypt {0}, AES 256 Bit, 2^{1} Rounds , {2} Ch..
        /// </summary>
        public string encrpytedStatusbarText
        {
            get
            {
                return Resources.encrpytedStatusbarText;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Hello World!.
        /// </summary>
        public string Hello_World
        {
            get
            {
                return Resources.Hello_World;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Edit.
        /// </summary>
        public string miEdit
        {
            get
            {
                return Resources.miEdit;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Copy.
        /// </summary>
        public string miEditCopy
        {
            get
            {
                return Resources.miEditCopy;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cut.
        /// </summary>
        public string miEditCut
        {
            get
            {
                return Resources.miEditCut;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Delete Line.
        /// </summary>
        public string miEditDeleteLine
        {
            get
            {
                return Resources.miEditDeleteLine;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Find....
        /// </summary>
        public string miEditFind
        {
            get
            {
                return Resources.miEditFind;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Find Next.
        /// </summary>
        public string miEditFindNext
        {
            get
            {
                return Resources.miEditFindNext;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Find Previous.
        /// </summary>
        public string miEditFindPrevious
        {
            get
            {
                return Resources.miEditFindPrevious;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Go To....
        /// </summary>
        public string miEditGoTo
        {
            get
            {
                return Resources.miEditGoTo;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Insert Date, Time.
        /// </summary>
        public string miEditInsertDateTime
        {
            get
            {
                return Resources.miEditInsertDateTime;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Paste.
        /// </summary>
        public string miEditPaste
        {
            get
            {
                return Resources.miEditPaste;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Redo.
        /// </summary>
        public string miEditRedo
        {
            get
            {
                return Resources.miEditRedo;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Replace....
        /// </summary>
        public string miEditReplace
        {
            get
            {
                return Resources.miEditReplace;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Select All.
        /// </summary>
        public string miEditSelectAll
        {
            get
            {
                return Resources.miEditSelectAll;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Undo.
        /// </summary>
        public string miEditUndo
        {
            get
            {
                return Resources.miEditUndo;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to File.
        /// </summary>
        public string miFile
        {
            get
            {
                return Resources.miFile;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Exit.
        /// </summary>
        public string miFileExit
        {
            get
            {
                return Resources.miFileExit;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Export Cleartext....
        /// </summary>
        public string miFileExportClearText
        {
            get
            {
                return Resources.miFileExportClearText;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Import With Encoding....
        /// </summary>
        public string miFileImportWithEnconding
        {
            get
            {
                return Resources.miFileImportWithEnconding;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to New.
        /// </summary>
        public string miFileNew
        {
            get
            {
                return Resources.miFileNew;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Open....
        /// </summary>
        public string miFileOpen
        {
            get
            {
                return Resources.miFileOpen;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Print....
        /// </summary>
        public string miFilePrint
        {
            get
            {
                return Resources.miFilePrint;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Save.
        /// </summary>
        public string miFileSave
        {
            get
            {
                return Resources.miFileSave;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Save As....
        /// </summary>
        public string miFileSaveAs
        {
            get
            {
                return Resources.miFileSaveAs;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Format.
        /// </summary>
        public string miFormat
        {
            get
            {
                return Resources.miFormat;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Spellchecking.
        /// </summary>
        public string miFormatCheckSpelling
        {
            get
            {
                return Resources.miFormatCheckSpelling;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Font....
        /// </summary>
        public string miFormatFont
        {
            get
            {
                return Resources.miFormatFont;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Word Wrap.
        /// </summary>
        public string miFormatWordWrap
        {
            get
            {
                return Resources.miFormatWordWrap;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Help.
        /// </summary>
        public string miHelp
        {
            get
            {
                return Resources.miHelp;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to About VisualCrypt....
        /// </summary>
        public string miHelpAbout
        {
            get
            {
                return Resources.miHelpAbout;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Log....
        /// </summary>
        public string miHelpLog
        {
            get
            {
                return Resources.miHelpLog;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View Help Online.
        /// </summary>
        public string miHelpViewOnline
        {
            get
            {
                return Resources.miHelpViewOnline;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to VisualCrypt.
        /// </summary>
        public string miVC
        {
            get
            {
                return Resources.miVC;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Password....
        /// </summary>
        public string miVCChangePassword
        {
            get
            {
                return Resources.miVCChangePassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypt.
        /// </summary>
        public string miVCDecrypt
        {
            get
            {
                return Resources.miVCDecrypt;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypt.
        /// </summary>
        public string miVCEncrypt
        {
            get
            {
                return Resources.miVCEncrypt;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Set Password....
        /// </summary>
        public string miVCSetPassword
        {
            get
            {
                return Resources.miVCSetPassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Settings....
        /// </summary>
        public string miVCSettings
        {
            get
            {
                return Resources.miVCSettings;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to View.
        /// </summary>
        public string miView
        {
            get
            {
                return Resources.miView;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Language.
        /// </summary>
        public string miViewLanguage
        {
            get
            {
                return Resources.miViewLanguage;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Status Bar.
        /// </summary>
        public string miViewStatusBar
        {
            get
            {
                return Resources.miViewStatusBar;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Tool Area.
        /// </summary>
        public string miViewToolArea
        {
            get
            {
                return Resources.miViewToolArea;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Zoom In.
        /// </summary>
        public string miViewZoomIn
        {
            get
            {
                return Resources.miViewZoomIn;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Zoom ({0}%).
        /// </summary>
        public string miViewZoomLevelText
        {
            get
            {
                return Resources.miViewZoomLevelText;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Zoom Out.
        /// </summary>
        public string miViewZoomOut
        {
            get
            {
                return Resources.miViewZoomOut;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Incorrect password or corrupted/forged message..
        /// </summary>
        public string msgPasswordError
        {
            get
            {
                return Resources.msgPasswordError;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypting:.
        /// </summary>
        public string operationDecryption
        {
            get
            {
                return Resources.operationDecryption;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypting the file being opened:.
        /// </summary>
        public string operationDecryptOpenedFile
        {
            get
            {
                return Resources.operationDecryptOpenedFile;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypting and saving:.
        /// </summary>
        public string operationEncryptAndSave
        {
            get
            {
                return Resources.operationEncryptAndSave;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypting:.
        /// </summary>
        public string operationEncryption
        {
            get
            {
                return Resources.operationEncryption;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ln {0}, Col {1} | Pos {2}/{3}.
        /// </summary>
        public string plaintextStatusbarPositionInfo
        {
            get
            {
                return Resources.plaintextStatusbarPositionInfo;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Plaintext | {0} | {1}.
        /// </summary>
        public string plaintextStatusbarText
        {
            get
            {
                return Resources.plaintextStatusbarText;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cancel.
        /// </summary>
        public string termCancel
        {
            get
            {
                return Resources.termCancel;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Copy to Clipboard.
        /// </summary>
        public string termCopyToClipboard
        {
            get
            {
                return Resources.termCopyToClipboard;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypt.
        /// </summary>
        public string termDecrypt
        {
            get
            {
                return Resources.termDecrypt;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypt.
        /// </summary>
        public string termEncrypt
        {
            get
            {
                return Resources.termEncrypt;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Password.
        /// </summary>
        public string termPassword
        {
            get
            {
                return Resources.termPassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Save.
        /// </summary>
        public string termSave
        {
            get
            {
                return Resources.termSave;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Set Password.
        /// </summary>
        public string termSetPassword
        {
            get
            {
                return Resources.termSetPassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://visualcrypt.com.
        /// </summary>
        public string uriHelpUrl
        {
            get
            {
                return Resources.uriHelpUrl;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://visualcrypt.com.
        /// </summary>
        public string uriPWSpecUrl
        {
            get
            {
                return Resources.uriPWSpecUrl;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://visualcrypt.com.
        /// </summary>
        public string uriSourceUrl
        {
            get
            {
                return Resources.uriSourceUrl;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://visualcrypt.com.
        /// </summary>
        public string uriSpecUrl
        {
            get
            {
                return Resources.uriSpecUrl;
            }
        }


        #endregion

        /// <summary>
        ///   Looks up a localized string similar to Discard Changes?.
        /// </summary>
        public  string msgDiscardChanges
        {
            get
            {
                return Resources.msgDiscardChanges;
            }
        }
    }
}
