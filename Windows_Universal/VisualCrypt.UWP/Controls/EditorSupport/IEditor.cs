using Windows.UI.Xaml.Controls;

namespace VisualCrypt.UWP.Controls.EditorSupport
{
    public interface IEditor
	{
		TextBox TextBox1 { get; }

		TextBox TextBoxFind { get; }

		TextBox TextBoxFindReplace { get; }

		TextBox TextBoxGoTo { get; }

		UserControl EditorControl { get; }
	}
}