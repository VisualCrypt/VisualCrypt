using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace VisualCrypt.Cryptography.Net.APIV2.Implementations
{
    public class Deflate
    {
        public byte[] Compress(string text, Encoding encoding)
        {
            if(text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            var bytes = encoding.GetBytes(text);

            using (var inputStream = new MemoryStream(bytes))
            using (var outputStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(outputStream, CompressionMode.Compress))
                {
                    inputStream.CopyTo(deflateStream);
                }
                return outputStream.ToArray();
            }
        }

        public string Decompress(byte[] bytes, Encoding encoding)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            using (var inputStream = new MemoryStream(bytes))
            using (var outputStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    deflateStream.CopyTo(outputStream);
                }
                return encoding.GetString(outputStream.ToArray());
            }
        }
    }
}
