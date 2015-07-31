﻿using System;
using System.Diagnostics;

namespace VisualCrypt.Cryptography.Portable.APIV2.Implementations
{
	public static class Base64Encoder
	{
		/// <summary>
		/// Converts a 8-bit unsigned integer array to an equivalent subset of a Unicode character array encoded with base-64 digits. 
		/// </summary>
		public static char[] EncodeDataToBase64CharArray(byte[] inputBytes)
		{
			if (inputBytes == null)
				throw new ArgumentNullException("inputBytes");

			var estimatedOutputCharCount = EstimateBase64EncodedLengthInChars(inputBytes.Length);
			var outputChars = new char[estimatedOutputCharCount];

			//  actualOutputCharCount: A 32-bit signed integer containing the number of chars () in outArray.
			var actualOutputCharCount = Convert.ToBase64CharArray(inputBytes, 0, inputBytes.Length, outputChars, 0);

			Debug.Assert(actualOutputCharCount == estimatedOutputCharCount);

			return outputChars;
		}

		/// <summary>
		/// Decodes a Base64 string from the editor to its binary form.
		/// </summary>
		public static byte[] DecodeBase64StringToBinary(string base64EncodedBinary)
		{
			return Convert.FromBase64String(base64EncodedBinary);
		}

		static int EstimateBase64EncodedLengthInChars(int rawBinaryLength)
		{
			// Each 3 byte sequence in the source data becomes a 4 char (not byte) 
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