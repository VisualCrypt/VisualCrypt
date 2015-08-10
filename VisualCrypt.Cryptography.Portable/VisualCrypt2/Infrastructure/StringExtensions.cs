using System;
using System.Text.RegularExpressions;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public static class StringExtensions
	{
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