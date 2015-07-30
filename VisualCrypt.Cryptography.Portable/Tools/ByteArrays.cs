using System;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Cryptography.Portable.Tools
{
	public static class ByteArrays
	{
		public static byte[] Concatenate(params byte[][] byteArrays)
		{
			var retLenght = 0;
			foreach (var byteArray in byteArrays)
			{
				if(byteArray == null)
					throw new ArgumentNullException("byteArrays","One of the byteArrays to be concatenated is null");
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
