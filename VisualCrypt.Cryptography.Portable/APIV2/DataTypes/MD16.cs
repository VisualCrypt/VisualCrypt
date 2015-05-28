using System;
using System.Linq;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
    public sealed class MD16
    {
        /// <summary>
        /// The non-null Value.
        /// </summary>
        public readonly byte[] Value;

        public MD16(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != 16)
                throw new ArgumentOutOfRangeException("value", "The lenght must be 16 bytes.");

            var allBytesZero = value.All(b => b == 0);

            if (allBytesZero)
                throw new ArgumentException("The hash must not have all bytes zero.", "value");

            Value = value;
        }
    }
}
