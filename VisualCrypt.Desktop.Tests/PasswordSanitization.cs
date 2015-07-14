using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.Portable.APIV2.Implementations;

namespace VisualCrypt.Desktop.Tests
{
	[TestClass]
	public class PasswordSanitization
	{
		[TestMethod]
		public void FilterControlCharacters()
		{
			var filtered = "\bHello\b\bWorld\b".FilterNonWhitespaceControlCharacters();
			Assert.AreEqual("HelloWorld", filtered);
		}

		[TestMethod]
		public void CondenseWhiteSpace()
		{
			var condensed = "Hello  World".CondenseWhiteSpace();
			Assert.AreEqual("Hello World", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceMultipleAndDifferent()
		{
			var condensed = "Hello \t \t\tWorld".CondenseWhiteSpace();
			Assert.AreEqual("Hello World", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceAtTheBeginning()
		{
			var condensed = "\t\tWorld".CondenseWhiteSpace();
			Assert.AreEqual(" World", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceAtTheEnd()
		{
			var condensed = "World  \t".CondenseWhiteSpace();
			Assert.AreEqual("World ", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceEmpty()
		{
			var condensed = "".CondenseWhiteSpace();
			Assert.AreEqual("", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceCRLF()
		{
			var condensed = "Hello\r\nWorld".CondenseWhiteSpace();
			Assert.AreEqual("Hello World", condensed);
		}
	}
}
