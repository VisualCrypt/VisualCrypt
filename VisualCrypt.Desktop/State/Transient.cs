using System.Text;
using VisualCrypt.Desktop.Features;

namespace VisualCrypt.Desktop.State
{
    /// <summary>
    /// Tracks the transient state of the editor.
    /// </summary>
    public class Transient
    {
        public FileModel FileModel { get; set; }

        /// <summary>
        ///     The current directory name, or the default location. Always use Path.Combine
        ///     for concatenating filename and the directory name.
        /// </summary>
        public string CurrentDirectoryName { get; set; }

    }
}