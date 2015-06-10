using System.Globalization;

namespace VisualCrypt.Cryptography.Portable.Tools
{
	public static class StringFormatInvariant
	{
		public static string FormatInvariant(this string formatString, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, formatString, args);
		}
	}
}