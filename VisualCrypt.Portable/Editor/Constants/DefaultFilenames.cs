using System;
using VisualCrypt.Portable.APIV2.DataTypes;
using VisualCrypt.Portable.Tools;

namespace VisualCrypt.Portable.Editor.Constants
{
    /// <summary>
    /// Default filenames for the invariant culture.
    /// </summary>
    public static class DefaultFilenames
    {
        /// <summary>
        /// Default filename for ContentKind.Text.
        /// </summary>
        const string UntitledText = "Untitled.txt.visualcrypt";

        /// <summary>
        /// Default filename for ContentKind.VCText.
        /// </summary>
        const string UntitledVisualCryptText = "Untitled.visualcrypt";

        /// <summary>
        /// Default filename for ContentKind.VCBinary.
        /// </summary>
        const string UntitledVisualCryptBinary = "Untitled.visualcrypt";

        /// <summary>
        /// Returns a default filename by ContentKind in the active locale.
        /// </summary>
        public static string GetDefaultFilename(ContentKind contentKind)
        {
            // todo: localize the app
            switch (contentKind)
            {
                case ContentKind.PlainText:
                    return UntitledText;
                case ContentKind.EncryptedText:
                    return UntitledVisualCryptText;
                case ContentKind.EncryptedBinary:
                    return UntitledVisualCryptBinary;
                default:
                    throw new ArgumentException("No DefaultFilename for ContentKind {0}".FormatInvariant(contentKind));
            }
        }

        public static string AdjustFileNameAfterDecryption(string fileName)
        {
            if (fileName == null)
                return null;
            // default: no changes
            string newFileName = fileName;

            if (!newFileName.EndsWith(".txt.visualcrypt", StringComparison.OrdinalIgnoreCase)
                && fileName.EndsWith(".visualcrypt", StringComparison.OrdinalIgnoreCase))
            {
                newFileName = fileName
                    .Remove(fileName.Length - 12, 12);
                newFileName += ".txt.visualcrypt";
            }
            if (!newFileName.EndsWith(".visualcrypt", StringComparison.OrdinalIgnoreCase))
            {
                newFileName += ".visualcrypt";
            }
            return newFileName;
        }

        public static string AdjustFileNameAfterEncryption(string fileName)
        {
            if (fileName == null)
                return null;

            // default: no changes
            string newFileName = fileName;

            // but remove .txt if present
            if (newFileName.EndsWith(".txt.visualcrypt", StringComparison.OrdinalIgnoreCase))
            {
                newFileName = fileName
                   .Remove(fileName.Length - 16, 16);
                newFileName += ".visualcrypt";
            }

            if (!newFileName.EndsWith(".visualcrypt", StringComparison.OrdinalIgnoreCase))
            {
                newFileName += ".visualcrypt";
            }

               
            return newFileName;
        }

       
    }
}
