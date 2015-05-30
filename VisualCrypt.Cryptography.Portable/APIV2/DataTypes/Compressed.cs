using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class Compressed
	{
		/// <summary>
		/// The non-null DataBytes.
		/// </summary>
		public readonly byte[] Value;

		public Compressed(byte[] value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			Value = value;
		}
	}
}