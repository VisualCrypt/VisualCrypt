using System;

namespace VisualCrypt.Portable.Tools
{
    public static class ByteArrayExtensions
    {
        public static void OverwriteWithZeros(this byte[] byteArray)
        {
            if(byteArray == null)
                throw new ArgumentNullException("byteArray");

            for (var index = 0; index < byteArray.Length; index++)
            {
                byteArray[index] = 0;
            }
        }
    }
}
