using VisualCrypt.Cryptography.Portable.Apps.MVVM;

namespace VisualCrypt.Cryptography.Portable.Apps.Models
{
    public class FileReference : ViewModelBase
    {
        public FileReference()
        {
            Filename = string.Empty;
            DirectoryName = string.Empty;
        }

        public string Filename { get; set; }

        public string DirectoryName { get; set; }



   
    }
}