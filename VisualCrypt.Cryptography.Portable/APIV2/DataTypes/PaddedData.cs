using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class PaddedData : SecureBytes
	{
		public PlainTextPadding PlainTextPadding
		{
			get { return _plainTextPadding; }
		}
		readonly PlainTextPadding _plainTextPadding;

		public PaddedData(byte[] dataBytes, PlainTextPadding plainTextPadding)
			: base(dataBytes)
		{
			// perform datatype-specific validation here
			if (plainTextPadding == null)
				throw new ArgumentNullException("plainTextPadding");

			_plainTextPadding =  plainTextPadding;
		}
	}
}