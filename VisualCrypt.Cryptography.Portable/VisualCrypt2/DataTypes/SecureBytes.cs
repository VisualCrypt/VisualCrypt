using System;
using System.Linq;
using System.Runtime.InteropServices;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.DataTypes
{
	public class SecureBytes
	{
		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public byte[] GetBytes()
		{
			return _data;
		}

		readonly byte[] _data;
		readonly GCHandle _gcHandle;

		protected SecureBytes(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			var allBytesZero = data.All(b => b == 0);

			if (allBytesZero)
				throw new ArgumentException("The hash must not have all bytes zero.", "data");

			_data = data;
			_gcHandle = GCHandle.Alloc(_data, GCHandleType.Pinned);
		}

		~SecureBytes()
		{
			_data.FillWithZeros();
			// ReSharper disable once ImpureMethodCallOnReadonlyValueField
			// ok, one-time call in finalizer
			_gcHandle.Free();
		}
	}
}