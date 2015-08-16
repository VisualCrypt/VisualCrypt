using System;

namespace VisualCrypt.Language
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
			if(loc == null)
				throw new ArgumentNullException("loc");

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
