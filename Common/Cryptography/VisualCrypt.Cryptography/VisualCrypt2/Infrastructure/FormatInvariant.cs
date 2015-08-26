using System.Globalization;

namespace VisualCrypt.Cryptography.VisualCrypt2.Infrastructure
{
	internal static class StringFormatInvariant
	{
		public static string FormatInvariant(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}
	}
}