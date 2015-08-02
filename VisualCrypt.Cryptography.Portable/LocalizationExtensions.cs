using System.Collections.Generic;

namespace VisualCrypt.Cryptography.Portable
{

	public static class Localization
	{
		static Dictionary<string, int> _languageOrder = new Dictionary<string, int>();

		static string _currentLocale = "EN";

		public static string GetLocalizedString(this string[] translations)
		{
			return translations[_languageOrder[_currentLocale]];
		}

		public static void SetLanguageOrder(string[] orderedLocales)
		{
			for (var i = 0; i < orderedLocales.Length; i++)
			{
				_languageOrder.Add(orderedLocales[i], i);
			}
		}

		public static void SetCurrentLocale(string locale)
		{
			_currentLocale = locale;
		}

		public static string GetCurrentLocale()
		{
			return _currentLocale;
		}
	}
}
