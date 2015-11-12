using Windows.UI;
using Windows.UI.Xaml.Media;

namespace VisualCrypt.Windows.Styles
{
    static class MoreColors
    {
        public static SolidColorBrush BackgroundGridDisabledColorBrush => new SolidColorBrush(Color.FromArgb(0xff, 0x40, 0x40, 0x40));

        public static SolidColorBrush AccentColorBrush => new SolidColorBrush(Color.FromArgb(0xff, 0xaf, 0x00, 0x07));

        public static SolidColorBrush WhiteBrush => new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff));

    }
}
