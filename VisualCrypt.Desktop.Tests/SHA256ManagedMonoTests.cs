using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Portable.APIV2.Implementations;
using VisualCrypt.Portable.Tools;

namespace VisualCrypt.Desktop.Tests
{
    [TestClass]
    public class SHA256ManagedMonoTests
    {
        [TestMethod]
        public void Test_Mono_Hash_Equals_Microsoft_SHA256()
        {
            var t0 = string.Format("some passphrase that might be used by 1000 people^^ ...{0}", DateTime.Now);
            var t1 = Guid.NewGuid().ToString();
            var t2 = string.Empty;

            var rnd = new Random();
            var buffer = new byte[1024];
            rnd.NextBytes(buffer);
            var t3 = ByteArrayToHexString.ByteArrayToHexViaLookup32(buffer);
            Test_Mono_Hash_Equals_Microsoft_SHA256_Core(t0);
            Test_Mono_Hash_Equals_Microsoft_SHA256_Core(t1);
            Test_Mono_Hash_Equals_Microsoft_SHA256_Core(t2);
            Test_Mono_Hash_Equals_Microsoft_SHA256_Core(t3);
        }

        void Test_Mono_Hash_Equals_Microsoft_SHA256_Core(string hashTestString)
        {
            var utf16LEPasswordBytes = Encoding.Unicode.GetBytes(hashTestString);

            byte[] hashMono;
            using (var sha256 = new SHA256ManagedMono())
            {
                hashMono = sha256.ComputeHash(utf16LEPasswordBytes);
            }

            byte[] hashMicrosoft;
            using (var sha256 = new SHA256Managed())
            {
                hashMicrosoft = sha256.ComputeHash(utf16LEPasswordBytes);
            }

            Assert.IsTrue(hashMicrosoft.Length * 8 == 256);
            Assert.IsTrue(hashMono.SequenceEqual(hashMicrosoft));
        }
    }
}
