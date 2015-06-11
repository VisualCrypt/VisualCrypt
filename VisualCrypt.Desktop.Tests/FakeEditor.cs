using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisualCrypt.Desktop.Shared.PrismSupport;

namespace VisualCrypt.Desktop.Tests
{
	class FakeEditor : IEditor
	{
		readonly TextBox _textBox1 = new TextBox();
		public System.Windows.Controls.TextBox TextBox1
		{
			get { return _textBox1; }
		}

		public System.Windows.Controls.TextBox TextBoxFind
		{
			get { throw new NotImplementedException(); }
		}

		public System.Windows.Controls.TextBox TextBoxFindReplace
		{
			get { throw new NotImplementedException(); }
		}

		public System.Windows.Controls.TextBox TextBoxGoTo
		{
			get { throw new NotImplementedException(); }
		}

		public System.Windows.UIElement EditorControl
		{
			get { throw new NotImplementedException(); }
		}
	}
}
