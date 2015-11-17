
namespace VisualCrypt.Applications.Models
{
    public class FilesPageCommandArgs
    {
        public FilesPageCommand FilesPageCommand;
        public FileReference FileReference { get; set; }
        public string TextContents { get; set; }
    }
}
