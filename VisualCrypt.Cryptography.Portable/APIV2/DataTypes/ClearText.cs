using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
    public sealed class ClearText
    {
        public const int MaxClearTextLenght = 1024 * 1024 * 10;

        public const int MinClearTextLenght = 0;

        /// <summary>
        /// The non-null DataBytes.
        /// </summary>
        public readonly string Value;

        public ClearText(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length > MaxClearTextLenght)
                throw new ArgumentOutOfRangeException("value", "Text too long.");

            if (value.Length < MinClearTextLenght)
                throw new ArgumentOutOfRangeException("value", string.Format("Text with a lenght shorter than {0} is not supported.", MinClearTextLenght));

            Value = value;
        }
    }
}
