using VisualCrypt.Cryptography.Portable.MVVM;

namespace VisualCrypt.Windows.Models
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