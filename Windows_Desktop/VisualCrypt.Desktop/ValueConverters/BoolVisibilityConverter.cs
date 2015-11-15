using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisualCrypt.Desktop.ValueConverters
{
	public class BoolVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            var visibility = (bool)value ? Visibility.Visible : Visibility.Collapsed;
            return visibility;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}