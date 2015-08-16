using System;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public static class ByteArrayFillWithZeros
	{
		public static void FillWithZeros(this byte[] byteArray)
		{
			if (byteArray == null)
				throw new ArgumentNullException("byteArray");

			for (var index = 0; index < byteArray.Length; index++)
			{
				byteArray[index] = 0;
			}
		}
	}
}