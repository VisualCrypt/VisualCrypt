using System.Linq;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.Portable.Tools;
using VisualCrypt.Desktop.ModuleEditor.Views;

namespace VisualCrypt.Desktop.Tests
{
	[TestClass]
	public class FindReplaceTests
	{
		readonly TextBox _textBox1 = new TextBox();

		[TestMethod]
		public void ReplaceAll_One_With_One()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 100));

			vm.FindString = "a";
			vm.ReplaceString = "b";
			vm.ReplaceAllCommand.Execute();
			var results = CountChars('b');

			Assert.IsTrue(results == 100, "Expecting 100 times 'b' but found {0} times 'b'".FormatInvariant(results));
		}

		[TestMethod]
		public void ReplaceAll_One_With_Two_Of_Same()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 100));

			vm.FindString = "a";
			vm.ReplaceString = "aa";
			vm.ReplaceAllCommand.Execute();

			var results = CountChars('a');

			Assert.IsTrue(results == 200, "Expecting 200 times 'a' but found {0} times 'a'".FormatInvariant(results));
		}

		[TestMethod]
		public void ReplaceAll_Two_With_One_Of_Same()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 100));

			vm.FindString = "aa";
			vm.ReplaceString = "a";
			vm.ReplaceAllCommand.Execute();

			var results = CountChars('a');

			Assert.IsTrue(results == 50, "Expecting 50 times 'a' but found {0} times 'a'".FormatInvariant(results));
		}

		[TestMethod]
		public void ReplaceAll_Two_With_One_Of_Same_Odd_Total()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 3));

			vm.FindString = "aa";
			vm.ReplaceString = "a";
			vm.ReplaceAllCommand.Execute();

			var results = CountChars('a');

			Assert.IsTrue(results == 2, "Expecting 2 times 'a' but found {0} times 'a'".FormatInvariant(results));
		}

		[TestMethod]
		public void FindNext()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 3));

			vm.FindString = "a";

			// search forward with wrapping
			vm.SearchOptions.SearchUp = false;

			_textBox1.CaretIndex = 0;
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 0);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 1);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 0);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 1);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2);

			// search backwards with wrapping
			vm.SearchOptions.SearchUp = true;
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 1);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 0);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 1);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 0);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2);
		}

		[TestMethod]
		public void FindNext_Long_UnEven()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 9));

			vm.FindString = "aa";

			// search forward with wrapping
			vm.SearchOptions.SearchUp = false;

			_textBox1.CaretIndex = 0;
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 0);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 4);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 6);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 0);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2);

			// search backwards with wrapping
			vm.SearchOptions.SearchUp = true;
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 0);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 7);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 5);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 3);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 1);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 7);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 5);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 3);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 1);
			vm.FindNextCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 7);
		}

		[TestMethod]
		public void ReplaceNext()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 4));

			vm.FindString = "a";
			vm.ReplaceString = "b";

			// search forward with wrapping
			vm.SearchOptions.SearchUp = false;

			_textBox1.CaretIndex = 0;
			vm.ReplaceCommand.Execute();
			vm.ReplaceCommand.Execute();
			vm.ReplaceCommand.Execute();
			vm.ReplaceCommand.Execute();

			var results = CountChars('b');
			Assert.IsTrue(results == 4);
		}

		[TestMethod]
		public void ReplaceNext_Shrink()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 6));

			vm.FindString = "aaa";
			vm.ReplaceString = "b";

			// search forward with wrapping
			vm.SearchOptions.SearchUp = false;

			_textBox1.CaretIndex = 0;
			vm.ReplaceCommand.Execute();
			vm.ReplaceCommand.Execute();


			var results_b = CountChars('b');
			Assert.IsTrue(results_b == 2);

			var results_a = CountChars('a');
			Assert.IsTrue(results_a == 0);
		}

		[TestMethod]
		public void ReplaceNext_Extend()
		{
			var vm = CreateViewModel();
			FillWithText("aabbbcc");

			vm.FindString = "b";
			vm.ReplaceString = "aaaa";

			// search forward with wrapping
			vm.SearchOptions.SearchUp = false;

			_textBox1.CaretIndex = 0;
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2);
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 6);
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 10);

			vm.FindString = "aaaaaaaaaaaaaa";
			vm.ReplaceString = "z";
			vm.ReplaceCommand.Execute();

			var results_z = CountChars('z');
			Assert.IsTrue(results_z == 1);

			Assert.IsTrue(_textBox1.CaretIndex == 0);
		}

		[TestMethod]
		public void ReplaceNext_Extend_SearchUp()
		{
			var vm = CreateViewModel();
			FillWithText("aabbbcc");

			vm.FindString = "b";
			vm.ReplaceString = "aaaa";

			// search forward with wrapping
			vm.SearchOptions.SearchUp = true;

			_textBox1.CaretIndex = 0;
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 4); // is 4
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 3); // sb 3
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_textBox1.CaretIndex == 2); // sb 2

			vm.FindString = "aaaaaaaaaaaaaa";
			vm.ReplaceString = "z";
			vm.ReplaceCommand.Execute();

			var results_z = CountChars('z');
			Assert.IsTrue(results_z == 1);

			Assert.IsTrue(_textBox1.CaretIndex == 0);
		}

		int CountChars(char character)
		{
			return (from c in _textBox1.Text
				where c == character
				select c).Count();
		}

		FindReplaceViewModel CreateViewModel()
		{
			var vm = new FindReplaceViewModel();
			vm.SetTextBox(_textBox1);
			vm.MessageBoxService = new FakeOkMessageBoxService();
			return vm;
		}

		void FillWithText(string text)
		{
			_textBox1.Text = text;
		}
	}
}