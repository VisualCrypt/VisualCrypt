using System;

namespace VisualCrypt.Desktop.Shared.App
{
    public static class Constants
    {
        // ReSharper disable InconsistentNaming
        internal const string Key_VisualCrypt = @"Software\VisualCrypt";     // root key
        internal const string Key_HasRunOnce = "HasRunOnce";                 // 0: has not run once, 1: has run once
        internal const string Key_NotepadSettings = "NotepadSettings";       // string key containing the serialized settings
        // ReSharper restore InconsistentNaming

        public const string ProductName = "VisualCrypt Notepad";

        public const string HelpUrl = "http://visualcrypt.com";
      
        public const string UntitledDotVisualCrypt = "Untitled.visualcrypt";
       
        public const string DotVisualCrypt = ".visualcrypt";
      
        public static readonly string DefaultDirectoryName;

        static Constants()
        {
            DefaultDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
    }
}
