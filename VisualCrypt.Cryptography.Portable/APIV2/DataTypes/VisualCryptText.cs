using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
    public sealed class VisualCryptText
    {
        /// <summary>
        /// The non-null DataBytes.
        /// </summary>
        public readonly string Value;

        public VisualCryptText(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Value = value;
        }
    }
}
