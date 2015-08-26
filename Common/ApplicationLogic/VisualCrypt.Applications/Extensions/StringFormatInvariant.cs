using System.Globalization;

namespace VisualCrypt.Applications.Extensions
{
	public static class StringFormatInvariant
	{
		public static string FormatInvariant(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}
	}
}