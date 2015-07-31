using System.Globalization;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public static class StringFormatInvariant
	{
		public static string FormatInvariant(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}
	}
}