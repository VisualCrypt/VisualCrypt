using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Windows.Controls
{
    class EditorViewModel
    {
        TextBox _textBox1;

        public void OnViewInitialized(TextBox textBox1)
        {
            Guard.NotNull(textBox1);
        }
    }
}
