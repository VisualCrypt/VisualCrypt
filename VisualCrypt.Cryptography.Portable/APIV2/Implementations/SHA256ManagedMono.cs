using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.Implementations
{
    public class SHA256ManagedMono : IDisposable
    {
        const int BlockSizeBytes = 64;

        // SHA-256 Constants
        // Represent the first 32 bits of the fractional parts of the
        // cube roots of the first sixty-four prime numbers
        readonly static uint[] K1 = {
            0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5,
            0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
            0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3,
            0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
            0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC,
            0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
            0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7,
            0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
            0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13,
            0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
            0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3,
            0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
            0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5,
            0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
            0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208,
            0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2
        };

        byte[] _hashValue;
        bool _disposed;
        ulong _count;
        int _processingBufferCount; // Counts how much data we have stored that still needs processed.

        readonly uint[] _h;
        readonly byte[] _processingBuffer;   // Used to start data when passed less than a block worth.
        readonly uint[] _buff;

        public SHA256ManagedMono()
        {
            _h = new uint[8];
            _processingBuffer = new byte[BlockSizeBytes];
            _buff = new uint[64];
            Initialize();
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return ComputeHash(buffer, 0, buffer.Length);
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("HashAlgorithm");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "< 0");
            if (count < 0)
                throw new ArgumentException("< 0", "count");
            // ordered to avoid possible integer overflow
            if (offset > buffer.Length - count)
            {
                throw new ArgumentException("Overflow: offset + count","offset");
            }

            HashCore(buffer, offset, count);
            _hashValue = HashFinal();
            Initialize();

            return _hashValue;
        }

        public void Dispose()
        {
            _disposed = true;
        }

		

	

        
       


      

     

        protected  void HashCore(byte[] rgb, int ibStart, int cbSize)
        {
            int i;

            if (_processingBufferCount != 0)
            {
                if (cbSize < (BlockSizeBytes - _processingBufferCount))
                {
                    Buffer.BlockCopy(rgb, ibStart, _processingBuffer, _processingBufferCount, cbSize);
                    _processingBufferCount += cbSize;
                    return;
                }
                i = (BlockSizeBytes - _processingBufferCount);
                Buffer.BlockCopy(rgb, ibStart, _processingBuffer, _processingBufferCount, i);
                ProcessBlock(_processingBuffer, 0);
                _processingBufferCount = 0;
                ibStart += i;
                cbSize -= i;
            }

            for (i = 0; i < cbSize - cbSize % BlockSizeBytes; i += BlockSizeBytes)
            {
                ProcessBlock(rgb, ibStart + i);
            }

            if (cbSize % BlockSizeBytes != 0)
            {
                Buffer.BlockCopy(rgb, cbSize - cbSize % BlockSizeBytes + ibStart, _processingBuffer, 0, cbSize % BlockSizeBytes);
                _processingBufferCount = cbSize % BlockSizeBytes;
            }
        }

        protected  byte[] HashFinal()
        {
            var hash = new byte[32];
            int i;

            ProcessFinalBlock(_processingBuffer, 0, _processingBufferCount);

            for (i = 0; i < 8; i++)
            {
                int j;
                for (j = 0; j < 4; j++)
                {
                    hash[i * 4 + j] = (byte)(_h[i] >> (24 - j * 8));
                }
            }

            return hash;
        }

        public void Initialize()
        {
            _count = 0;
            _processingBufferCount = 0;

            _h[0] = 0x6A09E667;
            _h[1] = 0xBB67AE85;
            _h[2] = 0x3C6EF372;
            _h[3] = 0xA54FF53A;
            _h[4] = 0x510E527F;
            _h[5] = 0x9B05688C;
            _h[6] = 0x1F83D9AB;
            _h[7] = 0x5BE0CD19;
        }

        void ProcessBlock(byte[] inputBuffer, int inputOffset)
        {
            uint a, b, c, d, e, f, g, h;
            uint t1, t2;
            int i;
            uint[] k1 = K1;
            uint[] buff = _buff;

            _count += BlockSizeBytes;

            for (i = 0; i < 16; i++)
            {
                buff[i] = (uint)(((inputBuffer[inputOffset + 4 * i]) << 24)
                    | ((inputBuffer[inputOffset + 4 * i + 1]) << 16)
                    | ((inputBuffer[inputOffset + 4 * i + 2]) << 8)
                    | ((inputBuffer[inputOffset + 4 * i + 3])));
            }


            for (i = 16; i < 64; i++)
            {
                t1 = buff[i - 15];
                t1 = (((t1 >> 7) | (t1 << 25)) ^ ((t1 >> 18) | (t1 << 14)) ^ (t1 >> 3));

                t2 = buff[i - 2];
                t2 = (((t2 >> 17) | (t2 << 15)) ^ ((t2 >> 19) | (t2 << 13)) ^ (t2 >> 10));
                buff[i] = t2 + buff[i - 7] + t1 + buff[i - 16];
            }

            a = _h[0];
            b = _h[1];
            c = _h[2];
            d = _h[3];
            e = _h[4];
            f = _h[5];
            g = _h[6];
            h = _h[7];

            for (i = 0; i < 64; i++)
            {
                t1 = h + (((e >> 6) | (e << 26)) ^ ((e >> 11) | (e << 21)) ^ ((e >> 25) | (e << 7))) + ((e & f) ^ (~e & g)) + k1[i] + buff[i];

                t2 = (((a >> 2) | (a << 30)) ^ ((a >> 13) | (a << 19)) ^ ((a >> 22) | (a << 10)));
                t2 = t2 + ((a & b) ^ (a & c) ^ (b & c));
                h = g;
                g = f;
                f = e;
                e = d + t1;
                d = c;
                c = b;
                b = a;
                a = t1 + t2;
            }

            _h[0] += a;
            _h[1] += b;
            _h[2] += c;
            _h[3] += d;
            _h[4] += e;
            _h[5] += f;
            _h[6] += g;
            _h[7] += h;
        }

        void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ulong total = _count + (ulong)inputCount;
            int paddingSize = (56 - (int)(total % BlockSizeBytes));

            if (paddingSize < 1)
                paddingSize += BlockSizeBytes;

            byte[] fooBuffer = new byte[inputCount + paddingSize + 8];

            for (int i = 0; i < inputCount; i++)
            {
                fooBuffer[i] = inputBuffer[i + inputOffset];
            }

            fooBuffer[inputCount] = 0x80;
            for (int i = inputCount + 1; i < inputCount + paddingSize; i++)
            {
                fooBuffer[i] = 0x00;
            }

            // I deal in bytes. The algorithm deals in bits.
            ulong size = total << 3;
            AddLength(size, fooBuffer, inputCount + paddingSize);
            ProcessBlock(fooBuffer, 0);

            if (inputCount + paddingSize + 8 == 128)
            {
                ProcessBlock(fooBuffer, 64);
            }
        }

        static void AddLength(ulong length, byte[] buffer, int position)
        {
            buffer[position++] = (byte)(length >> 56);
            buffer[position++] = (byte)(length >> 48);
            buffer[position++] = (byte)(length >> 40);
            buffer[position++] = (byte)(length >> 32);
            buffer[position++] = (byte)(length >> 24);
            buffer[position++] = (byte)(length >> 16);
            buffer[position++] = (byte)(length >> 8);
            buffer[position] = (byte)(length);
        }

        
    }
}