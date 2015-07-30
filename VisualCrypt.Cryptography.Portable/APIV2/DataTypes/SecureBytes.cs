using System;
using System.Linq;
using System.Runtime.InteropServices;
using VisualCrypt.Cryptography.Portable.Tools;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public class SecureBytes
	{
		/// <summary>
		/// Guaranteed to be non-null.
		/// </summary>
		public byte[] DataBytes
		{
			get { return _dataBytes; }
		}
		readonly byte[] _dataBytes;
		readonly GCHandle _gcHandle;

		protected SecureBytes(byte[] dataBytes)
		{
			if (dataBytes == null)
				throw new ArgumentNullException("dataBytes");

			var allBytesZero = dataBytes.All(b => b == 0);

			if (allBytesZero)
				throw new ArgumentException("The hash must not have all bytes zero.", "dataBytes");

			_dataBytes = dataBytes;
			_gcHandle = GCHandle.Alloc(_dataBytes, GCHandleType.Pinned);
		}

		~SecureBytes()
		{
			_dataBytes.FillWithZeros();
			_gcHandle.Free();
		}
	}
}
