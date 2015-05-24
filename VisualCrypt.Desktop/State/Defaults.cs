using System;
using System.Text;

namespace VisualCrypt.Desktop.State
{
    public class Defaults
    {
        public const string ProductName = "VisualCrypt Notepad";

        public const string HelpUrl = "http://visualcrypt.com";
      
        public const string UntitledDotVisualCrypt = "Untitled.visualcrypt";
       
        public const string VisualCryptExtension = ".visualcrypt";
      
        public readonly string DefaultDirectoryName;

        public Defaults()
        {
            DefaultDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
    }
}
