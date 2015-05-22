﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VisualCrypt.Portable.APIV2.DataTypes;
using VisualCrypt.Portable.APIV2.Interfaces;

namespace VisualCrypt.Net.APIV2.Implementations
{
    public class CoreAPIV2_Net4 : ICoreAPIV2
    {
        public Compressed Compress(ClearText clearText)
        {
            if (clearText == null)
                throw new ArgumentNullException("clearText");

            var deflate = new Deflate();
            byte[] compressed = deflate.Compress(clearText.Value, Encoding.UTF8);

            return new Compressed(compressed);
        }

        public PaddedData ApplyRandomPadding(Compressed compressed)
        {
            if(compressed == null)
                throw new ArgumentNullException("compressed");

            if (compressed.Value.Length == 0)
                return new PaddedData(compressed.Value, 0);

            var requiredPadding = 16 - compressed.Value.Length % 16;
            var paddingBytes = new byte[requiredPadding];

            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(paddingBytes);

            var paddedDataBytes = new byte[compressed.Value.Length + requiredPadding];
            Buffer.BlockCopy(compressed.Value, 0, paddedDataBytes, 0, compressed.Value.Length);
            Buffer.BlockCopy(paddingBytes, 0, paddedDataBytes, compressed.Value.Length, paddingBytes.Length);

            return new PaddedData(paddedDataBytes, requiredPadding);
        }

        public IV16 GenerateIV(int lenght)
        {
            var iv = new byte[lenght];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
                return new IV16(iv);
            }
        }

        public CipherV2 AESEncryptMessage(PaddedData paddedData, AESKey32 aesKey32, IV16 iv16)
        {
            if (paddedData == null)
                throw new ArgumentNullException("paddedData");

            if (aesKey32 == null)
                throw new ArgumentNullException("aesKey32");

            if (iv16 == null)
                throw new ArgumentNullException("iv16");

            var aes = new AesManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Padding = PaddingMode.None,
                IV = iv16.Value,
                Key = aesKey32.Value,
                Mode = CipherMode.CBC
            };



            var cipher = new CipherV2 { Padding = paddedData.Padding, IV16 = iv16 };

            using (var stream = new MemoryStream())
            using (var encryptor = aes.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(paddedData.DataBytes, 0, paddedData.DataBytes.Length);
                encrypt.FlushFinalBlock();
                cipher.CipherBytes = stream.ToArray();

                return cipher;
            }
        }

        public void AESEncryptMessageDigest(CipherV2 cipherv2, MD16 md16, AESKey32 aesKey32)
        {
            if (cipherv2 == null)
                throw new ArgumentNullException("cipherv2");

            if (md16 == null)
                throw new ArgumentNullException("md16");

            if (aesKey32 == null)
                throw new ArgumentNullException("aesKey32");

            var aes = new AesManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Padding = PaddingMode.None,
                IV = cipherv2.IV16.Value,
                Key = aesKey32.Value,
                Mode = CipherMode.CBC
            };


            using (var stream = new MemoryStream())
            using (var encryptor = aes.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(md16.Value, 0, md16.Value.Length);
                encrypt.FlushFinalBlock();
                cipherv2.MD16E = new MD16E(stream.ToArray());
            }
        }

        public MD16 AESDecryptMessageDigest(MD16E md16E, IV16 iv16, AESKey32 aesKey32)
        {
            if (md16E == null)
                throw new ArgumentNullException("md16E");

            if (iv16 == null)
                throw new ArgumentNullException("iv16");

            if (aesKey32 == null)
                throw new ArgumentNullException("aesKey32");

            var aes = new AesManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Padding = PaddingMode.None,
                IV = iv16.Value,
                Key = aesKey32.Value,
                Mode = CipherMode.CBC
            };

            using (var stream = new MemoryStream())
            using (var decryptor = aes.CreateDecryptor())
            using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                decrypt.Write(md16E.Value, 0, md16E.Value.Length);
                decrypt.FlushFinalBlock();
                return new MD16(stream.ToArray());
            }
        }

        public PaddedData AESDecryptMessage(CipherV2 cipherV2, IV16 iv16, AESKey32 aesKey32)
        {
            if (cipherV2 == null)
                throw new ArgumentNullException("cipherV2");

            if (iv16 == null)
                throw new ArgumentNullException("iv16");

            if (aesKey32 == null)
                throw new ArgumentNullException("aesKey32");

            var aes = new AesManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Padding = PaddingMode.None,
                IV = iv16.Value,
                Key = aesKey32.Value,
                Mode = CipherMode.CBC
            };

            using (var stream = new MemoryStream())
            using (var decryptor = aes.CreateDecryptor())
            using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                decrypt.Write(cipherV2.CipherBytes, 0, cipherV2.CipherBytes.Length);
                decrypt.FlushFinalBlock();
                return new PaddedData(stream.ToArray(), cipherV2.Padding);
            }
        }

        public Compressed RemovePadding(PaddedData paddedData)
        {
            if (paddedData == null)
                throw new ArgumentNullException("paddedData");

            var paddingRemoved = new byte[paddedData.DataBytes.Length - paddedData.Padding];

            Buffer.BlockCopy(paddedData.DataBytes,0,paddingRemoved,0,paddingRemoved.Length);

            return new Compressed(paddingRemoved);
        }

        public ClearText Decompress(Compressed compressed)
        {
            if(compressed == null)
                throw new ArgumentNullException("compressed");

            var deflate = new Deflate();
            var clearText = deflate.Decompress(compressed.Value, Encoding.UTF8);
            return new ClearText(clearText);
        }
    }
}
