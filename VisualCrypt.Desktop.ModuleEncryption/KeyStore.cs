﻿using System;
using VisualCrypt.Cryptography.Portable.APIV2.DataTypes;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Desktop.ModuleEncryption
{
	internal static class KeyStore
	{
		static SHA512PW64 _sha512PW64;

		public static SHA512PW64 GetSHA256PW32()
		{
			if (_sha512PW64 == null)
				throw new InvalidOperationException("KeyStore: _sha512PW64 is null.");
			return _sha512PW64;
		}

		public static void SetSHA256PW32(SHA512PW64 sha512PW64)
		{
			_sha512PW64 = sha512PW64;
		}

		public static void Clear()
		{
			if (_sha512PW64 != null)
				_sha512PW64.Value.FillWithZeros();
			_sha512PW64 = null;
		}
	}
}