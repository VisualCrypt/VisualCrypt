using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;

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
			var condensed = "Hello  World".CondenseAndNormalizeWhiteSpace();
			Assert.AreEqual("Hello World", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceMultipleAndDifferent()
		{
			var condensed = "Hello \t \t\tWorld".CondenseAndNormalizeWhiteSpace();
			Assert.AreEqual("Hello World", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceAtTheBeginning()
		{
			var condensed = "\t\tWorld".CondenseAndNormalizeWhiteSpace();
			Assert.AreEqual(" World", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceAtTheEnd()
		{
			var condensed = "World  \t".CondenseAndNormalizeWhiteSpace();
			Assert.AreEqual("World ", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceEmpty()
		{
			var condensed = "".CondenseAndNormalizeWhiteSpace();
			Assert.AreEqual("", condensed);
		}

		[TestMethod]
		public void CondenseWhiteSpaceCRLF()
		{
			var condensed = "Hello\r\nWorld".CondenseAndNormalizeWhiteSpace();
			Assert.AreEqual("Hello World", condensed);
		}
	}
}
