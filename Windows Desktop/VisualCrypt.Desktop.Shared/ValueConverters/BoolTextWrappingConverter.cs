﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisualCrypt.Desktop.Shared.ValueConverters
{
	public class BoolTextWrappingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value ? TextWrapping.Wrap : TextWrapping.NoWrap;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var textWrapping = (TextWrapping) value;
			if (textWrapping == TextWrapping.NoWrap)
				return false;
			return true;
		}
	}
}