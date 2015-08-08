using VisualCrypt.Windows.Models;

namespace VisualCrypt.Windows.Pages
{
    class FilesPageCommandArgs
    {
        public FilesPageCommand FilesPageCommand;
        public FileReference FileReference { get; set; }
    }

    enum FilesPageCommand
    {
        Invalid = 0, New, Open
    }
}
