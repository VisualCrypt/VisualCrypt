using System.Globalization;

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
    }
}
