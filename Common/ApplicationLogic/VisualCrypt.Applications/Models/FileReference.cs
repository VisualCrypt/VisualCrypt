namespace VisualCrypt.Applications.Models
{
    public class FileReference : ViewModelBase
    {
        public FileReference()
        {
            ShortFilename = string.Empty;
            PathAndFileName = string.Empty;
            ModifiedDate = string.Empty;
        }

        public string ShortFilename { get; set; }

        public string PathAndFileName { get; set; }

        public string ModifiedDate { get; set; }

        public object FileSystemObject { get; set; }



   
    }
}