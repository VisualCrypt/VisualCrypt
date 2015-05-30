using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace VisualCrypt.Desktop.Shared
{
	public static class StringExtensions
	{
		/// <summary>
		/// Formats using the Invariant Culture.
		/// </summary>
		/// <param name="formatString">Format string</param>
		/// <param name="args">Arguments</param>
		/// <exception cref="System.ArgumentNullException">System.ArgumentNullException</exception>
		/// <exception cref="System.FormatException">System.FormatException</exception>
		public static string FormatInvariant(this string formatString, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, formatString, args);
		}

		public static string ReplaceCaseInsensitive(this string str, string from, string to)
		{
			if (str == null)
				return null;
			if (from == null)
				return str;
			if (to == null)
				throw new ArgumentNullException("to");
			return Regex.Replace(str, from, to, RegexOptions.IgnoreCase);
		}
	}
}