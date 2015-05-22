using System;
using System.Linq;


namespace VisualCrypt.Portable.APIV2.DataTypes
{
    public sealed class SHA256PW32
    {
        /// <summary>
        /// The non-null Value.
        /// </summary>
        public readonly byte[] Value;

        public SHA256PW32(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != 32)
                throw new ArgumentOutOfRangeException("value", "The lenght must be 32 bytes.");

            var allBytesZero = value.All(b => b == 0);

            if (allBytesZero)
                throw new ArgumentException("The hash must not have all bytes zero.", "value");

            Value = value;
        }
    }
}
