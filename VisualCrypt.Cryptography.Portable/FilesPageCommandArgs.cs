
using VisualCrypt.Cryptography.Portable.Models;

namespace VisualCrypt.Cryptography.Portable
{
    public class FilesPageCommandArgs
    {
        public FilesPageCommand FilesPageCommand;
        public FileReference FileReference { get; set; }
    }

    public enum FilesPageCommand
    {
        Invalid = 0, New, Open
    }
}
