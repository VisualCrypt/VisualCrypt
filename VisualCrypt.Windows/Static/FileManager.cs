using System;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;

namespace VisualCrypt.Windows.Static
{
    static class FileManager
    {
        static FileManager ()
        {
            FileModel = FileModel.EmptyCleartext();
        }

        public static BindableFileInfo BindableFileInfo = new BindableFileInfo();

        public static FileModel FileModel { get; set; }

        internal static void ShowEncryptedBar()
        {
            
        }

        internal static void ShowWorkingBar(string description)
        {
            
        }

        internal static void ShowPlainTextBar()
        {
            
        }
    }
}
