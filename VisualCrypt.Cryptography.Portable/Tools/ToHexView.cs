using System;
using System.Collections.Generic;
using System.Text;

namespace VisualCrypt.Cryptography.Portable.Tools
{
	public static class ByteArrayToHexView
	{
		const char Dot = '.';
		const char Spacer = ' ';
		const int BytesPerLine = 16;
		static readonly char[] ASCIIChars = CreateASCIITable();
		static readonly uint[] HexTable = CreateHexTable();

		public static string ToHexView(this IEnumerable<byte> bytes)
		{
			if(bytes == null)
				throw new ArgumentNullException("bytes");

			var allLines = new StringBuilder();
			var asciiLine = new StringBuilder();
			var hexLine = new StringBuilder();

			var bytesInLine = 0;
			foreach (var currentByte in bytes)
			{
				var val = HexTable[currentByte];
				hexLine.Append((char)val);
				hexLine.Append((char)(val >> 16));
				hexLine.Append(Spacer);

				asciiLine.Append(ASCIIChars[currentByte]);
				bytesInLine++;

				if (bytesInLine == BytesPerLine)
				{
					allLines.Append(hexLine);
					allLines.AppendLine(asciiLine.ToString());
					hexLine.Clear();
					asciiLine.Clear();
					bytesInLine = 0;
				}
			}
			return allLines.ToString();
		}

		static char[] CreateASCIITable()
		{
			var encoding = Encoding.UTF8;
			var asciiChars = new Char[256];
			var index = 0;
			for (int i = 0; i < asciiChars.Length; i++)
			{
				if (index > 126 || index < 32)
					asciiChars[index] = Dot;
				else
					asciiChars[index] = encoding.GetChars(new[] {(byte) index})[0];
				index++;
			}
			return asciiChars;
		}

		static uint[] CreateHexTable()
		{
			var result = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				string s = i.ToString("X2");
				result[i] = s[0] + ((uint)s[1] << 16);
			}
			return result;
		}
	}
}