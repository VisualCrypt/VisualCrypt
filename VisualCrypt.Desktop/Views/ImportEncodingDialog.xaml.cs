using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace VisualCrypt.Desktop.Views
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ImportEncodingDialog
    {
        EncodingInfo _selectedEncodingInfo;

        public ImportEncodingDialog()
        {
            InitializeComponent();
            DataContext = this;
            PreviewKeyDown += CloseWithEscape;
        }

        void CloseWithEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public IEnumerable<EncodingInfo> AvailableEncodings
        {
            get
            {
                var encodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName).ToList();
                var defaultEncoding = encodings.SingleOrDefault(e => e.DisplayName == Encoding.Default.EncodingName);
                if (defaultEncoding != null)
                    SelectedEncodingInfo = defaultEncoding;

                return encodings;
            }
        }

        public EncodingInfo SelectedEncodingInfo
        {
            get { return _selectedEncodingInfo; }
            set { _selectedEncodingInfo = value; }
        }


        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}