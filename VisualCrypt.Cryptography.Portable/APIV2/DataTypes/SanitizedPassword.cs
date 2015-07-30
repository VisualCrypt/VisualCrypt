using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class SanitizedPassword
	{
		public const int MaxSanitizedPasswordLength = 1000000;

		const int MinSanitizedPasswordLength = 0;

		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public readonly string StringValue;

		public SanitizedPassword(string stringValue)
		{
			if (stringValue == null)
				throw new ArgumentNullException("stringValue");

			if (stringValue.Length > MaxSanitizedPasswordLength)
				throw new ArgumentOutOfRangeException("stringValue", string.Format("The password is {0} characters too long.", stringValue.Length - MaxSanitizedPasswordLength));

			if (stringValue.Length < MinSanitizedPasswordLength)
				throw new ArgumentOutOfRangeException("stringValue",
					string.Format("Passwords with a length shorter than {0} are not supported.", MinSanitizedPasswordLength));

			StringValue = stringValue;
		}
	}
}