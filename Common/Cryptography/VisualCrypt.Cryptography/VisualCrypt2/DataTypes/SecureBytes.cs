using System;
using System.Runtime.InteropServices;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
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

			var nonZeroBytesPresent = false;
			foreach (var b in data)
			{
				if (b != 0)
				{
					nonZeroBytesPresent = true;
					break;
				}
			}

			if (data.Length >0 && !nonZeroBytesPresent)
				throw new ArgumentException("Invalid data: all bytes zero.", "data");

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