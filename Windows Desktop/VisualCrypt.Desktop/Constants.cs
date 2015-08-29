using System;

namespace VisualCrypt.Desktop
{
	public static class Constants
	{
		// ReSharper disable InconsistentNaming
		internal const string Key_VisualCrypt = @"Software\VisualCrypt"; // root key
		internal const string Key_HasRunOnce = "HasRunOnce"; // 0: has not run once, 1: has run once
		internal const string Key_EditorSettings = "EditorSettings"; // string key containing the serialized settings
        internal const string Key_FontSettings = "FontSettings";
        internal const string Key_CryptoSettings = "CryptoSettings";
       
        // ReSharper restore InconsistentNaming

        public const string ProductName = "VisualCrypt Editor";

		

		

		

		

		// Open/Save VisualCrypt
		public static string VisualCryptDialogFilter_DefaultExt = ".visualcrypt";

		public static string VisualCryptDialogFilter =
			"VisualCrypt (*.visualcrypt; *.txt)|*.visualcrypt;*.txt|All Files(*.*)|*.*";

		// Import/Export Plaintext
		public static string TextDialogFilter_DefaultExt = ".txt";
		public static string TextDialogFilter = "Text(*.txt)|*.txt|All Files(*.*)|*.*";
	    


	
	}
}