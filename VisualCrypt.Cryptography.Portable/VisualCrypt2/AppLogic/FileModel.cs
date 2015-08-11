using System.Text;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Language;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic
{
    public class FileModel : NotifyPropertyChanged, IFileModel
    {
        static readonly Encoding VisualCryptTextSaveEncoding = new UTF8Encoding(false, true);
        static readonly Encoding DefaultClearTextSaveEncoding = VisualCryptTextSaveEncoding;

        FileModel()
        {
        }


        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isDirty;


        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_filename != value)
                {
                    _filename = value;
                    OnPropertyChanged();
                }
            }
        }
        string _filename;

        public string ShortFilename
        {
            get { return _shortFilename; }
            set
            {
                if (_shortFilename != value)
                {
                    _shortFilename = value;
                    OnPropertyChanged();
                }
            }
        }
        string _shortFilename;


        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            private set
            {
                if (_isEncrypted != value)
                {
                    _isEncrypted = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isEncrypted;



        public string ClearTextContents
        {
            get { return _clearTextContents; }
            private set
            {
                if (_clearTextContents != value)
                {
                    _clearTextContents = value;
                    OnPropertyChanged();
                }
            }
        }
        string _clearTextContents;


        public string VisualCryptText
        {
            get { return _visualCryptText; }
            private set
            {
                if (_visualCryptText != value)
                {
                    _visualCryptText = value;
                    OnPropertyChanged();
                }
            }
        }
        string _visualCryptText;

        public string SaveEncodingName
        {
            get { return _saveEncodingName; }
            private set
            {
                if (_saveEncodingName != value)
                {
                    _saveEncodingName = value;
                    OnPropertyChanged();
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
                Filename = Loc.Strings.constUntitledDotVisualCrypt,
                ShortFilename = Loc.Strings.constUntitledDotVisualCrypt,
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
            IsEncrypted = source.IsDirty;
            CipherV2 = source.CipherV2;
            ClearTextContents = source.ClearTextContents;
            SaveEncoding = source.SaveEncoding;
            VisualCryptText = source.VisualCryptText;
        }
    }
}