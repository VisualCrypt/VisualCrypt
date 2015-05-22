using System.Security;
using System.Text;
using VisualCrypt.Portable.APIV2.DataTypes;

namespace VisualCrypt.Desktop.State
{
    /// <summary>
    /// Tracks the transient state of the editor.
    /// </summary>
    public class Transient
    {
        /// <summary>
        /// Initializes ALL transient properties with startup defaults.
        /// </summary>
        public Transient()
        {
            SHA256PW32 = null;
            ContentKind = ContentKind.PlainText;
            CurrentFilename = null;
            CurrentDirectoryName = ModelState.Defaults.DefaultDirectoryName;
            CanExit = true;
            CanSave = false;
        }


        /// <summary>
        /// Initial password
        /// </summary>
        public SHA256PW32 SHA256PW32 { get; set; }

        /// <summary>
        ///     Gives a hint what's probably in the editor.
        /// </summary>
        /// See
        /// <see cref="ContentKind" />
        /// class for legal values.
        public ContentKind ContentKind { get; set; }

        /// <summary>
        ///     The encoding that will be used when the user saves the editor contents
        ///     using Save As... or Save.
        ///     UTF8 with signature, fault tolerant.
        /// </summary>
        public Encoding SaveEncoding
        {
            get { return new UTF8Encoding(true, false); }
        }

        /// <summary>
        ///     The current filename (with extension, without path) that is used
        ///     in the title bar, when the Save As... dialog opens,
        ///     or used by Save, if CanSave returns true.
        /// </summary>
        public string CurrentFilename { get; set; }

        /// <summary>
        ///     The current directory name, or the default location. Always use Path.Combine
        ///     for concatenating filename and the directory name.
        /// </summary>
        public string CurrentDirectoryName { get; set; }

        /// <summary>
        ///     Whether the user can close the editor without being warned about a loss of data.
        ///     Semantically the inversion of 'IsDirty'.
        /// </summary>
        public bool CanExit { get; set; }

        /// <summary>
        ///     Whether the Save menu command is enabled.
        /// </summary>
        public bool CanSave { get; set; }

        public bool IsSHA256PasswordHashPresent { get; set; }
    }
}