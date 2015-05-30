using System.Linq;
using System.Security;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.Net.Tools;

namespace VisualCrypt.Desktop.Tests
{
	[TestClass]
	public class SecureStringTests
	{
		[TestMethod]
		public void CanExtractUTF16LEBytes()
		{
			const string text1 = "The bytes extracted from SecureString must be UTF16LE";
			var bytes1 = Encoding.Unicode.GetBytes(text1);

			var secure1 = new SecureString();
			var pos = 0;
			foreach (char c in text1)
			{
				secure1.InsertAt(pos, c);
				pos++;
			}
			var bytes2 = secure1.ToUTF16ByteArray();
			Assert.IsTrue(bytes1.SequenceEqual(bytes2));
		}
	}
}