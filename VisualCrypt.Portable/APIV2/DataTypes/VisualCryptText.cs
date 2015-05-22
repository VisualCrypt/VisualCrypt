using System;


namespace VisualCrypt.Portable.APIV2.DataTypes
{
    public sealed class VisualCryptText
    {
        public const string Prefix = "VisualCrypt/text";

        /// <summary>
        /// The non-null DataBytes.
        /// </summary>
        public readonly string Value;

        public VisualCryptText(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!value.StartsWith(Prefix))
                throw new ArgumentException("Invalid format.", "value");

            if (value.Length < Prefix.Length)
                throw new ArgumentOutOfRangeException("value", "Invalid lenght.");

            Value = value;
        }
    }
}
