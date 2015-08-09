using System;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations
{
	public static class KeyStore
	{
		static SHA512PW64 _sha512PW64;

		public static SHA512PW64 GetPasswordHash()
		{
			if (_sha512PW64 == null)
				throw new InvalidOperationException("KeyStore: _sha512PW64 is null.");
			return _sha512PW64;
		}

		public static void SetPasswordHash(SHA512PW64 sha512PW64)
		{
			_sha512PW64 = sha512PW64;
		}

		public static void Clear()
		{
			if (_sha512PW64 != null)
				_sha512PW64.GetBytes().FillWithZeros();
			_sha512PW64 = null;
		}
	}
}