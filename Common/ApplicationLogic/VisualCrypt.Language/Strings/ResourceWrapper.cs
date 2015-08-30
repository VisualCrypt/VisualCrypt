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

        /// <summary>
        ///   Looks up a localized string similar to Import with encoding: {0}.
        /// </summary>
        public  string titleImportWithEncoding
        {
            get
            {
                return  Resources.titleImportWithEncoding;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Password.
        /// </summary>
        public  string termChangePassword
        {
            get
            {
                return Resources.termChangePassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; could not be found..
        /// </summary>
        public  string msgFindCouldNotBeFound
        {
            get
            {
                return Resources.msgFindCouldNotBeFound;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to No match for &apos;{0}&apos; could be found..
        /// </summary>
        public  string msgFindRegExNoMatch
        {
            get
            {
                return Resources.msgFindRegExNoMatch;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Invalid Regular Expression Syntax.
        /// </summary>
        public string msgReplaceInvalidRegEx
        {
            get
            {
                return Resources.msgReplaceInvalidRegEx;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to {0} occurrences were replaced..
        /// </summary>
        public string msgReplaceOccucancesReplaced
        {
            get
            {
                return Resources.msgReplaceOccucancesReplaced;
            }
        }


        /// <summary>
        ///   Looks up a localized string similar to Replace All.
        /// </summary>
        public string termReplaceAll
        {
            get
            {
                return Resources.termReplaceAll;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Nothing found - Search again from the top of the document?.
        /// </summary>
       public string msgNothingFoundSearchFromStart
        {
            get
            {
                return Resources.msgNothingFoundSearchFromStart;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Binary File.
        /// </summary>
        public string termBinary
        {
            get
            {
                return Resources.termBinary;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to This file is neither text nor VisualCrypt - display with Hex View?\r\n\r\nIf file is very large the editor may become less responsive..
        /// </summary>
        public string msgFileIsBinary
        {
            get
            {
                return Resources.msgFileIsBinary;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Export Clear Text (Encoding: {0}).
        /// </summary>
        public string titleExportCleartext
        {
            get
            {
                return Resources.titleExportCleartext;
            }
        }

        public string spdm_Change_OK
        {
            get
            {
                return Resources.spdm_Change_OK;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Password.
        /// </summary>
        public string spdm_Change_Title
        {
            get
            {
                return Resources.spdm_Change_Title;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change Password and decrypt.
        /// </summary>
        public string spdm_CorrectPassword_OK
        {
            get
            {
                return Resources.spdm_CorrectPassword_OK;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The current password is not correct.
        /// </summary>
        public string spdm_CorrectPassword_Title
        {
            get
            {
                return Resources.spdm_CorrectPassword_Title;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Set Password.
        /// </summary>
        public string spdm_Set_OK
        {
            get
            {
                return Resources.spdm_Set_OK;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Set Password.
        /// </summary>
        public string spdm_Set_Title
        {
            get
            {
                return Resources.spdm_Set_Title;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypt.
        /// </summary>
        public string spdm_SetAndDecrypt_OK
        {
            get
            {
                return Resources.spdm_SetAndDecrypt_OK;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Set Password &amp; Decrypt.
        /// </summary>
        public string spdm_SetAndDecrypt_Title
        {
            get
            {
                return Resources.spdm_SetAndDecrypt_Title;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Decrypt loaded file.
        /// </summary>
        public string spdm_SetAndDecryptLoadedFile_OK
        {
            get
            {
                return Resources.spdm_SetAndDecryptLoadedFile_OK;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Enter password to decrypt loaded file.
        /// </summary>
        public string spdm_SetAndDecryptLoadedFile_Title
        {
            get
            {
                return Resources.spdm_SetAndDecryptLoadedFile_Title;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypt.
        /// </summary>
        public string spdm_SetAndEncrypt_OK
        {
            get
            {
                return Resources.spdm_SetAndEncrypt_OK;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Set Password &amp; Encrypt.
        /// </summary>
        public string spdm_SetAndEncrypt_Title
        {
            get
            {
                return Resources.spdm_SetAndEncrypt_Title;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Encrypt and Save.
        /// </summary>
        public string spdm_SetAndEncryptAndSave_OK
        {
            get
            {
                return Resources.spdm_SetAndEncryptAndSave_OK;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Set Password, Encrypt and Save.
        /// </summary>
        public string spdm_SetAndEncryptAndSave_Title
        {
            get
            {
                return Resources.spdm_SetAndEncryptAndSave_Title;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The password is effectively empty - are you sure?.
        /// </summary>
        public string spd_msgPasswordEffectivelyEmpty
        {
            get
            {
                return Resources.spd_msgPasswordEffectivelyEmpty;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Use empty password?.
        /// </summary>
        public string spd_msgUseEmptyPassword
        {
            get
            {
                return Resources.spd_msgUseEmptyPassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to {0} of {1} Unicode Characters.
        /// </summary>
        public string spd_msgXofYUnicodeChars
        {
            get
            {
                return Resources.spd_msgXofYUnicodeChars;
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Password/-phrase:.
        /// </summary>
        public string spd_lbl_PasswordOrPhrase
        {
            get
            {
                return Resources.spd_lbl_PasswordOrPhrase;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Generate Password.
        /// </summary>
        public string spd_linktext_GeneratePassword
        {
            get
            {
                return Resources.spd_linktext_GeneratePassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Print Password.
        /// </summary>
        public string spd_linktext_PrintPassword
        {
            get
            {
                return Resources.spd_linktext_PrintPassword;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to from 256 Bit Random Data.
        /// </summary>
        public string spd_text_from256BitRD
        {
            get
            {
                return Resources.spd_text_from256BitRD;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Read more about.
        /// </summary>
        public string spd_text_ReadMoreAbout
        {
            get
            {
                return Resources.spd_text_ReadMoreAbout;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cancel.
        /// </summary>
        public string termClear
        {
            get
            {
                return Resources.termClear;
            }
        }

        public string spd_linktext_VisualCryptPasswords
        {
            get
            {
                return Resources.spd_linktext_VisualCryptPasswords;
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to VisualCrypt 2 (AES 256 Bit, BCrypt - Multi).
        /// </summary>
        public string sett_combo_VisualCrypt2
        {
            get
            {
                return Resources.sett_combo_VisualCrypt2;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to source code.
        /// </summary>
        public string sett_linktext_Source
        {
            get
            {
                return Resources.sett_linktext_Source;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Spec.
        /// </summary>
        public string sett_linktext_Spec
        {
            get
            {
                return Resources.sett_linktext_Spec;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to BCrypt/AES Rounds:.
        /// </summary>
        public string sett_text_BCryptAESRounds
        {
            get
            {
                return Resources.sett_text_BCryptAESRounds;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Default: 2.
        /// </summary>
        public string sett_text_default_2_power
        {
            get
            {
                return Resources.sett_text_default_2_power;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to or view the.
        /// </summary>
        public string sett_text_orViewThe
        {
            get
            {
                return Resources.sett_text_orViewThe;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Read the.
        /// </summary>
        public string sett_text_ReadThe
        {
            get
            {
                return Resources.sett_text_ReadThe;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Warning: A high value will turn encryption and decryption into a very time consuming operation..
        /// </summary>
        public string sett_warn_high
        {
            get
            {
                return Resources.sett_warn_high;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Warning: A low value faciliates brute force and dictionary attacks..
        /// </summary>
        public string sett_warn_low
        {
            get
            {
                return Resources.sett_warn_low;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The setting influences the required computational work to create the BCrypt hash. A higher value means more work..
        /// </summary>
        public string sett_warn_neutral
        {
            get
            {
                return Resources.sett_warn_neutral;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Defaults.
        /// </summary>
        public string termDefaults
        {
            get
            {
                return Resources.termDefaults;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Defaults.
        /// </summary>
        public string sett_text_EncrpytionMethod
        {
            get
            {
                return Resources.sett_text_EncrpytionMethod;
            }
        }

    }
}
