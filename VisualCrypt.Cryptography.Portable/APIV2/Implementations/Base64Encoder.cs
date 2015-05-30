using System;
using System.Diagnostics;

namespace VisualCrypt.Cryptography.Portable.APIV2.Implementations
{
    public class Base64Encoder
    {
        /// <summary>
        /// Converts a 8-bit unsigned integer array to an equivalent subset of a Unicode character array encoded with base-64 digits. 
        /// </summary>
        public static char[] EncodeDataToBase64CharArray(byte[] rawBinaryData)
        {
            if(rawBinaryData == null)
                return new char[0];

            var rawBinaryLengthInBytes = rawBinaryData.Length;
            var base64CharArrayLengthInChars = EstimateBase64EncodedLengthInBytes(rawBinaryLengthInBytes);
            var base64CharArray = new char[base64CharArrayLengthInChars];

            //  A 32-bit signed integer containing the number of chars (not bytes!!!) in outArray.
            var anotherCount = Convert.ToBase64CharArray(inArray: rawBinaryData, offsetIn: 0, length: rawBinaryLengthInBytes, outArray: base64CharArray, offsetOut: 0);

            Debug.Assert(anotherCount == base64CharArrayLengthInChars);

            return base64CharArray;
        }

        /// <summary>
        /// Decodes a Base64 string from the editor to its binary form.
        /// </summary>
        public static byte[] DecodeBase64StringToBinary(string base64EncodedBinary)
        {
            var binaryBytes = Convert.FromBase64String(base64EncodedBinary);
            return binaryBytes;
        }

        static int EstimateBase64EncodedLengthInBytes(int rawBinaryLength)
        {
            // Convert the binary input into Base64 UUEncoded output. 
            // Each 3 byte sequence in the source data becomes a 4 char (not byte!!!) 
            // sequence in the character array.  
            int arrayLengthInChars = (int)((4.0d / 3.0d) * rawBinaryLength);

            // If array length is not divisible by 4, go up to the next 
            // multiple of 4. 
            if (arrayLengthInChars % 4 != 0)
            {
                arrayLengthInChars += 4 - arrayLengthInChars % 4;
            }

          
            return arrayLengthInChars;
        }
    }
}
