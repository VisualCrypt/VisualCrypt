using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;
using VisualCrypt.Desktop.ModuleEditor.Views;
using VisualCrypt.Desktop.Shared.PrismSupport;

namespace VisualCrypt.Desktop.Tests
{
	[TestClass]
	public class FindReplaceTests
	{
		readonly IEditor _editor = new FakeEditor();

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

			_editor.TextBox1.CaretIndex = 0;
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 1);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 1);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2);

			// search backwards with wrapping
			vm.SearchOptions.SearchUp = true;
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 1);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 1);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2);
		}

		[TestMethod]
		public void FindNext_Long_UnEven()
		{
			var vm = CreateViewModel();
			FillWithText(new string('a', 9));

			vm.FindString = "aa";

			// search forward with wrapping
			vm.SearchOptions.SearchUp = false;

			_editor.TextBox1.CaretIndex = 0;
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 4);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 6);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2);

			// search backwards with wrapping
			vm.SearchOptions.SearchUp = true;
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 7);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 5);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 3);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 1);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 7);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 5);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 3);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 1);
			vm.FindNextButtonCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 7);
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

			_editor.TextBox1.CaretIndex = 0;
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

			_editor.TextBox1.CaretIndex = 0;
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

			_editor.TextBox1.CaretIndex = 0;
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2);
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 6);
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 10);

			vm.FindString = "aaaaaaaaaaaaaa";
			vm.ReplaceString = "z";
			vm.ReplaceCommand.Execute();

			var results_z = CountChars('z');
			Assert.IsTrue(results_z == 1);

			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
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

			_editor.TextBox1.CaretIndex = 0;
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 4); // is 4
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 3); // sb 3
			vm.ReplaceCommand.Execute();
			Assert.IsTrue(_editor.TextBox1.CaretIndex == 2); // sb 2

			vm.FindString = "aaaaaaaaaaaaaa";
			vm.ReplaceString = "z";
			vm.ReplaceCommand.Execute();

			var results_z = CountChars('z');
			Assert.IsTrue(results_z == 1);

			Assert.IsTrue(_editor.TextBox1.CaretIndex == 0);
		}

		int CountChars(char character)
		{
			return (from c in _editor.TextBox1.Text
				where c == character
				select c).Count();
		}

		EditorViewModel CreateViewModel()
		{

			var vm = ServiceLocator.Current.GetInstance<EditorViewModel>();
			vm._messageBoxService = new FakeOkMessageBoxService();
			vm.OnEditorLoaded(_editor);
			return vm;
		}

		void FillWithText(string text)
		{
			_editor.TextBox1.Text = text;
		}
	}
}
