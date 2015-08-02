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
		}
	}
}
