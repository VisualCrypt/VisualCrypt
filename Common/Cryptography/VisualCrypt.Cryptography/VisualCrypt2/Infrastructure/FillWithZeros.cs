using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.Infrastructure
{
	public static class ByteArrayFillWithZeros
	{
		public static void FillWithZeros(this byte[] byteArray)
		{
            // this check is important because this method may be called from within
            // a finalizer where the supplied bytearray might already be garbage-collected.
            if (byteArray == null)
                return; 

			for (var index = 0; index < byteArray.Length; index++)
			{
				byteArray[index] = 0;
			}
		}
	}
}