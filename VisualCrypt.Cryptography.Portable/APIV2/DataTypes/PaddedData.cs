using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
    public sealed class PaddedData
    {
        /// <summary>
        /// The non-null DataBytes.
        /// </summary>
        public readonly byte[] DataBytes;

        public readonly byte Padding;

        public PaddedData(byte[] dataBytes, int padding)
        {
            if (dataBytes == null)
                throw new ArgumentNullException("dataBytes");

            if (padding < 0 || padding > 15)
                throw new ArgumentException("The padding amount must be >= 0 and < 16", "padding");

            DataBytes = dataBytes;
            Padding = (byte)padding;
        }
    }
}
