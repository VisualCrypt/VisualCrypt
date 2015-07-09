using System;

namespace VisualCrypt.Desktop.Shared.App
{
	public static class Constants
	{
		// ReSharper disable InconsistentNaming
		internal const string Key_VisualCrypt = @"Software\VisualCrypt"; // root key
		internal const string Key_HasRunOnce = "HasRunOnce"; // 0: has not run once, 1: has run once
		internal const string Key_NotepadSettings = "NotepadSettings"; // string key containing the serialized settings
		// ReSharper restore InconsistentNaming

		public const string ProductName = "VisualCrypt Editor";

		public const string HelpUrl = "http://visualcrypt.com/Help/Windows/VisualCrypt2";
		public const string SpecUrl = "http://visualcrypt.com/Specs/VisualCrypt2";
		public const string SourceUrl = "https://github.com/VisualCrypt/VisualCrypt-Editor";

		public const string UntitledDotVisualCrypt = "Untitled.visualcrypt";

		public const string DotVisualCrypt = ".visualcrypt";

		public static readonly string DefaultDirectoryName;

		// Open/Save VisualCrypt
		public static string VisualCryptDialogFilter_DefaultExt = ".visualcrypt";

		public static string VisualCryptDialogFilter =
			"VisualCrypt (*.visualcrypt; *.txt)|*.visualcrypt;*.txt|All Files(*.*)|*.*";

		// Import/Export Plaintext
		public static string TextDialogFilter_DefaultExt = ".txt";
		public static string TextDialogFilter = "Text(*.txt)|*.txt|All Files(*.*)|*.*";


		static Constants()
		{
			DefaultDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}
	}
}