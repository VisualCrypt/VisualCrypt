using System;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.Portable
{
	public static class Loc
	{
		static Loc()
		{
			Strings = new Strings();
		}

		public static Strings Strings { get; private set; }

		public static void SetLanguage(string loc)
		{
			Guard.NotNull(loc);

			Strings.SwitchLocale(loc);

			OnLocaleChanged(new EventArgs());
		}

		public static event EventHandler LocaleChanged;

		 static void OnLocaleChanged(EventArgs e)
		{
			var handler = LocaleChanged;
			if (handler != null)
			{
				handler(null, e);
			}
		}
	}
}
