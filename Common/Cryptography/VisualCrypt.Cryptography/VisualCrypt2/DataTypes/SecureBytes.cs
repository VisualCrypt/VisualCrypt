using System;
using System.Runtime.InteropServices;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.VisualCrypt2.DataTypes
{
    public class SecureBytes
    {
        /// <summary>
        /// Returns a non-null clone of the array stored in the data type. 
        /// </summary>
        public byte[] GetBytes()
        {
            var cloned = new byte[_data.Length];
            Buffer.BlockCopy(_data, 0, cloned, 0, _data.Length);
            return cloned;
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

            if (data.Length > 0 && !nonZeroBytesPresent)
                throw new ArgumentException("Invalid data: all bytes zero.", "data");

            _data = data;
            _gcHandle = GCHandle.Alloc(_data, GCHandleType.Pinned);
        }

        ~SecureBytes()
        {
            try
            {
                if (_data != null)
                    _data.FillWithZeros();
                if (_gcHandle.IsAllocated)
                    _gcHandle.Free();
            }
            catch (InvalidOperationException)
            { }
        }
    }
}