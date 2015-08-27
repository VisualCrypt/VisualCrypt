using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;

namespace VisualCrypt.Windows
{
    class FontSettings
    {
        public FontFamily FontFamily { get; internal set; }
        public double FontSize { get; internal set; }
        public FontStretch FontStretch { get; internal set; }
        public FontStyle FontStyle { get; internal set; }
        public FontWeight FontWeight { get; internal set; }
    }
}
