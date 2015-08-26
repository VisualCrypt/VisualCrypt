namespace VisualCrypt.Applications.Models
{
    public class FileReference : ViewModelBase
    {
        public FileReference()
        {
            ShortFilename = string.Empty;
            Filename = string.Empty;
        }

        public string ShortFilename { get; set; }

        public string Filename { get; set; }



   
    }
}