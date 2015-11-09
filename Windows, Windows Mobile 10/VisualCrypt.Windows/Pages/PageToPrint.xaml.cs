using Windows.UI.Xaml.Controls;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class PageToPrint : Page
    {
        public RichTextBlock TextContentBlock { get; set; }

        public PageToPrint()
        {
            this.InitializeComponent();
            TextContentBlock = TextContent;
        }
    }
}
