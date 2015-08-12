using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisualCrypt.Desktop.Shared.ValueConverters
{
	public class IsHiddenVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}