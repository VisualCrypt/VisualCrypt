using System;
using System.Text;

namespace VisualCrypt.Desktop.State
{
    public class Defaults
    {
        public const string HelpUrl = "http://visualcrypt.com";
        public const string ProgramName = "VisualCrypt Notepad";
      
        public readonly string DefaultDirectoryName;
        public Encoding DefaultEncoding = Encoding.UTF8;

        public Defaults()
        {
            DefaultDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
    }
}
