﻿using System;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
	public sealed class NormalizedPassword
	{
		public const int MaxSanitizedPasswordLength = 1000000;

		const int MinSanitizedPasswordLength = 0;

		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public string Text
		{
			get { return _unprunedPassword; }
		}
		readonly string _unprunedPassword;

		public NormalizedPassword(string unprunedPassword)
		{
			if (unprunedPassword == null)
				throw new ArgumentNullException("unprunedPassword");

			if (unprunedPassword.Length > MaxSanitizedPasswordLength)
				throw new ArgumentOutOfRangeException("unprunedPassword",
					"The password is {0} characters too long.".FormatInvariant(unprunedPassword.Length - MaxSanitizedPasswordLength));

			if (unprunedPassword.Length < MinSanitizedPasswordLength)
				throw new ArgumentOutOfRangeException("unprunedPassword",
					"Passwords with a length shorter than {0} are not supported.".FormatInvariant(MinSanitizedPasswordLength));

			_unprunedPassword = unprunedPassword;
		}

		
	}
}