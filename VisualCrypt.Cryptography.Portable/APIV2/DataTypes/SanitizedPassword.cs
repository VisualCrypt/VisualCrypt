using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public sealed class SanitizedPassword
	{
		public const int MaxSanitizedPasswordLength = 1000000;

		public const int MinSanitizedPasswordLength = 0;

		/// <summary>
		/// The non-null SanitizedPassword.
		/// </summary>
		public readonly string Value;

		public SanitizedPassword(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			if (value.Length > MaxSanitizedPasswordLength)
				throw new ArgumentOutOfRangeException("value", string.Format("The password is {0} characters too long.", value.Length - MaxSanitizedPasswordLength));

			if (value.Length < MinSanitizedPasswordLength)
				throw new ArgumentOutOfRangeException("value",
					string.Format("Passwords with a length shorter than {0} are not supported.", MinSanitizedPasswordLength));

			Value = value;
		}
	}
}