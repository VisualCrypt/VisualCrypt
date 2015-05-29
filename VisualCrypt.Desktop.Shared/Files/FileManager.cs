using System;

namespace VisualCrypt.Desktop.Shared.Files
{
    public static class FileManager
    {
        public static readonly BindableFileInfo BindableFileInfo = new BindableFileInfo();
        static FileModelBase _fileModel = new CleartextFileModel();


        public static FileModelBase FileModel
        {
            get { return _fileModel; }
            set
            {
                if(value == null)
                    throw new ArgumentNullException("value");
                _fileModel = value;
                _fileModel.PropertyChanged += _fileModel_PropertyChanged;
                BindableFileInfo.IsDirty = _fileModel.IsDirty;
                BindableFileInfo.Filename = _fileModel.Filename;
                BindableFileInfo.IsEncrypted = _fileModel.IsEncrypted;
            }
        }

        static void _fileModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var fileModel = (FileModelBase)sender; 
            switch (e.PropertyName)
            {
                case "IsDirty":
                    BindableFileInfo.IsDirty = fileModel.IsDirty;
                    break;
                case "Filename":
                    BindableFileInfo.Filename= fileModel.Filename;
                    break;
                case "IsEncrypted":
                    BindableFileInfo.IsEncrypted = fileModel.IsEncrypted;
                    break;
                default:
                    throw new ArgumentException("Unknown property name {0}.", e.PropertyName);
            }
        }
    }
}
