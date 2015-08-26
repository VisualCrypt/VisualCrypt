using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VisualCrypt.Desktop.Views.Fonts
{
	internal class FontFamilyListItem : TextBlock, IComparable
	{
		readonly string _displayName;

		public FontFamilyListItem(FontFamily fontFamily)
		{
			_displayName = GetDisplayName(fontFamily);

			FontFamily = fontFamily;
			Text = _displayName;
			ToolTip = _displayName;

			// In the case of symbol font, apply the default message font to the text so it can be read.
			if (IsSymbolFont(fontFamily))
			{
				var range = new TextRange(ContentStart, ContentEnd);
				range.ApplyPropertyValue(FontFamilyProperty, SystemFonts.MessageFontFamily);
			}
		}

		int IComparable.CompareTo(object obj)
		{
			return string.Compare(_displayName, obj.ToString(), true, CultureInfo.CurrentCulture);
		}

		public override string ToString()
		{
			return _displayName;
		}

		internal static bool IsSymbolFont(FontFamily fontFamily)
		{
			foreach (Typeface typeface in fontFamily.GetTypefaces())
			{
				GlyphTypeface face;
				if (typeface.TryGetGlyphTypeface(out face))
				{
					return face.Symbol;
				}
			}
			return false;
		}

		internal static string GetDisplayName(FontFamily family)
		{
			return NameDictionaryHelper.GetDisplayName(family.FamilyNames);
		}
	}
}