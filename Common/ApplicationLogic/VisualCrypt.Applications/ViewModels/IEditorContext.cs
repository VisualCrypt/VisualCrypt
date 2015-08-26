using VisualCrypt.Applications.Models;

namespace VisualCrypt.Applications.ViewModels
{
    public interface IEditorContext
    {
        IFileModel FileModel { get; }
    }
}
