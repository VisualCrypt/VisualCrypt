using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable;

namespace VisualCrypt.Windows.Services
{
    class FileService : IFileService
    {
        public void WriteAllBytes(string filename, byte[] encodedTextBytes)
        {
            throw new NotImplementedException();
        }

        public bool CheckFilenameForQuickSave(string filename)
        {
            throw new NotImplementedException();
        }
    }
}
