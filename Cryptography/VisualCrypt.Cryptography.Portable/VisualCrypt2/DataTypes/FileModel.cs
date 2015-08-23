using System;
using System.Text;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
    public class FileModel :  IFileModel
    {
        

        static readonly Encoding VisualCryptTextSaveEncoding = new UTF8Encoding(false, true);
        static readonly Encoding DefaultClearTextSaveEncoding = VisualCryptTextSaveEncoding;

	    public Action<FileModel> OnFileModelUpdated;

        FileModel()
        {
        }


        public bool IsDirty { get; set; }



        public string Filename { get; set; }  // should the setter be private?


        public string ShortFilename { get; set; }  // should the setter be private?



        public bool IsEncrypted { get; private set; }




        public string ClearTextContents { get; private set; }
       


        public string VisualCryptText { get; private set; }
       

        public string SaveEncodingName 
        {
            get { return _saveEncodingName; }
            private set
            {
                if (_saveEncodingName != value)
                {
                    _saveEncodingName = value;
                }
            }
        }
        string _saveEncodingName = "ToDoEcoding";


        public Encoding SaveEncoding { get; private set; }

        public CipherV2 CipherV2 { get; private set; }


        public static FileModel EmptyCleartext()
        {
            return new FileModel
            {
                Filename = LocalizableStrings.UntitledDotVisualCrypt,
                ShortFilename = LocalizableStrings.UntitledDotVisualCrypt,
                ClearTextContents = string.Empty,
                IsEncrypted = false,
                SaveEncoding = DefaultClearTextSaveEncoding,
                VisualCryptText = "SetError: VisualCryptText is not valid in this context."
            };
        }

        public static FileModel Cleartext(string filename, string shortFilename, string clearTextContents, Encoding saveEncoding)
        {
            return new FileModel
            {
                Filename = filename,
                ShortFilename = shortFilename,
                ClearTextContents = clearTextContents,
                IsEncrypted = false,
                SaveEncoding = saveEncoding,
                VisualCryptText = "SetError: VisualCryptText is not valid in this context."
            };
        }

        public static FileModel Encrypted(object cipherV2, string filename, string shortFilename, string visualCryptText)
        {
            return new FileModel
            {
                Filename = filename,
                ShortFilename = shortFilename,
                ClearTextContents = "SetError: ClearTextContents is not valid in this context.",
                IsEncrypted = true,
                SaveEncoding = VisualCryptTextSaveEncoding,
                VisualCryptText = visualCryptText,
                CipherV2 = (CipherV2)cipherV2
            };
        }

        public void UpdateFrom(FileModel source)
        {
            ShortFilename = source.ShortFilename;
            Filename = source.Filename;
            IsDirty = source.IsDirty;
            IsEncrypted = source.IsEncrypted;
            CipherV2 = source.CipherV2;
            ClearTextContents = source.ClearTextContents;
            SaveEncoding = source.SaveEncoding;
            VisualCryptText = source.VisualCryptText;

	        OnFileModelUpdated(this);

        }


    }
}