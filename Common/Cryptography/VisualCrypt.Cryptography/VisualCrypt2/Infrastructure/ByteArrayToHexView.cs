using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VisualCrypt.Cryptography.VisualCrypt2.Infrastructure
{
    public static class ByteArrayToHexView
    {
        const char Dot = '.';
        const char Spacer = ' ';
        const int BytesPerLine = 16;
        static readonly char[] ASCIIChars = CreateASCIITable();
        static readonly uint[] HexTable = CreateHexTable();

        public static string ToHexView(this IEnumerable<byte> bytes, bool showASCII = true)
        {
            if (bytes == null)
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
                    if (showASCII)
                        allLines.AppendLine(asciiLine.ToString());
                    else
                        allLines.AppendLine();
                    hexLine.Clear();
                    asciiLine.Clear();
                    bytesInLine = 0;
                }
            }
            allLines.Append(hexLine);
            if (showASCII)
                allLines.AppendLine(asciiLine.ToString());
            else
                allLines.AppendLine();
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
                    asciiChars[index] = encoding.GetChars(new[] { (byte)index })[0];
                index++;
            }
            return asciiChars;
        }

        static uint[] CreateHexTable()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2", CultureInfo.InvariantCulture);
                result[i] = s[0] + ((uint)s[1] << 16);
            }
            return result;
        }
    }
}