using System;

namespace VisualCrypt.Cryptography.VisualCrypt2.Infrastructure
{
	public static class ByteArrays
	{
		public static byte[] Concatenate(params byte[][] byteArrays)
		{
			if(byteArrays == null)
				throw new ArgumentNullException("byteArrays");

			var retLenght = 0;
			foreach (var byteArray in byteArrays)
			{
				if (byteArray == null)
					throw new ArgumentNullException("byteArrays", "One of the byteArrays to be concatenated is null");
				retLenght += byteArray.Length;
			}

			var ret = new byte[retLenght];

			var offset = 0;
			foreach (var byteArray in byteArrays)
			{
				Buffer.BlockCopy(byteArray, 0, ret, offset, byteArray.Length);
				offset += byteArray.Length;
			}
			return ret;
		}
	}
}