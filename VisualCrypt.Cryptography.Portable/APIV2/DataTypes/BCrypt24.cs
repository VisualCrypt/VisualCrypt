using System;
using System.Linq;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class BCrypt24
	{
		/// <summary>
		/// The non-null DataBytes.
		/// </summary>
		public readonly byte[] Value;

		public BCrypt24(byte[] value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (value.Length != 24)
				throw new ArgumentOutOfRangeException("value", "The length must be 24 bytes.");

			var allBytesZero = value.All(b => b == 0);

			if (allBytesZero)
				throw new ArgumentException("The hash must not have all bytes zero.", "value");

			Value = value;
		}
	}
}