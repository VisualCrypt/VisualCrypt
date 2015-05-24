using System;
using System.Text;
using VisualCrypt.Portable.APIV2.DataTypes;
using VisualCrypt.Portable.Tools;

namespace VisualCrypt.Portable.APIV2.Implementations
{
    public class VisualCryptFormatter
    {
        public const string VisualCryptSlashBinary = "VisualCrypt/binary*";

        public const string VisualCryptSlashText = "VisualCrypt/text*";

        public VisualCryptText CreateVisualCryptText(CipherV2 cipherV2)
        {
            if (cipherV2 == null)
                throw new ArgumentNullException("cipherV2");

            var visualCryptTextV2Bytes = new byte[1 + 1 + 16 + 16 + cipherV2.CipherBytes.Length];

            visualCryptTextV2Bytes[0] = CipherV2.Version;
            visualCryptTextV2Bytes[1] = cipherV2.Padding;
            Buffer.BlockCopy(cipherV2.IV16.Value, 0, visualCryptTextV2Bytes, 2, 16);
            Buffer.BlockCopy(cipherV2.MD16E.Value, 0, visualCryptTextV2Bytes, 18, 16);
            Buffer.BlockCopy(cipherV2.CipherBytes, 0, visualCryptTextV2Bytes, 34, cipherV2.CipherBytes.Length);

            var visualCryptTextV2Base64 = Base64Encoder.EncodeDataToBase64CharArray(visualCryptTextV2Bytes);

            var sb = new StringBuilder();
            const int breakAfter = 74;
            var charsInLine = 0;

            foreach (var c in VisualCryptSlashText)
            {
                sb.Append(c);
                if (++charsInLine != breakAfter)
                    continue;
                sb.Append(new[] { '\r', '\n' });
                charsInLine = 0;
            }

            foreach (var c in visualCryptTextV2Base64)
            {
                sb.Append(c);
                if (++charsInLine != breakAfter)
                    continue;
                sb.Append(new[] { '\r', '\n' });
                charsInLine = 0;
            }

            return new VisualCryptText(sb.ToString());
        }

        public CipherV2 DissectVisualCryptText(VisualCryptText visualCryptText)
        {
            if (visualCryptText == null)
                throw new ArgumentNullException("visualCryptText");

            try
            {
                var visualCrypt = visualCryptText.Value.Trim();

                if (!visualCrypt.StartsWith(VisualCryptSlashText, StringComparison.OrdinalIgnoreCase))
                    throw new FormatException("The data is not in VisualCrypt/text V2 format (because it does not start with '{0}').".FormatInvariant(VisualCryptSlashText));

                var visualCryptTextV2Base64 = visualCrypt.Remove(0, VisualCryptSlashText.Length);

                var visualCryptTextV2Bytes = Base64Encoder.DecodeBase64StringToBinary(visualCryptTextV2Base64);

                if (visualCryptTextV2Bytes[0] != CipherV2.Version)
                    throw new FormatException("The data is not in VisualCrypt/text V2 format. Expected a version byte at index 0 of value '2'.");

                if (visualCryptTextV2Bytes[1] > 15)
                    throw new FormatException("The data is not in VisualCrypt/text V2 format. The value at the padding byte at index 1 is invalid.");

                var cipher = new CipherV2 { Padding = visualCryptTextV2Bytes[1] };


                var iv16 = new byte[16];
                Buffer.BlockCopy(visualCryptTextV2Bytes, 2, iv16, 0, 16);
                cipher.IV16 = new IV16(iv16);

                var md16E = new byte[16];
                Buffer.BlockCopy(visualCryptTextV2Bytes, 18, md16E, 0, 16);
                cipher.MD16E = new MD16E(md16E);

                var cipherBytes = new byte[visualCryptTextV2Bytes.Length - 34];
                Buffer.BlockCopy(visualCryptTextV2Bytes, 34, cipherBytes, 0, cipherBytes.Length);
                cipher.CipherBytes = cipherBytes;

                return cipher;
            }
            catch (Exception e)
            {
                throw new FormatException("Data invalid or truncated. " + e.Message);
            }
        }
    }
}
