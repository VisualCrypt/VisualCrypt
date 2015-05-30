using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Mvvm;
using VisualCrypt.Desktop.Shared;
using VisualCrypt.Desktop.Shared.App;

namespace VisualCrypt.Desktop.Views
{
	public partial class ImportEncoding
	{
		EncodingInfo _selectedEncodingInfo;


		public ImportEncoding()
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
				var encodings = Encoding.GetEncodings().OrderBy((e) => e.DisplayName).ToList();
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