using System;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.Portable.Apps.Models;
using VisualCrypt.Cryptography.Portable.Apps.Services;

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

        public bool Exists(string filename)
        {
            throw new NotImplementedException();
        }

        public string ReadAllText(string filename, Encoding selectedEncoding)
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter, DialogDirection dialogDirection, string title = null)
        {
            throw new NotImplementedException();
        }
    }
}
