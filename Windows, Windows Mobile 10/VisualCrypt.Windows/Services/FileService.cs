using System;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Windows.Services
{
    class FileService : IFileService
    {
        public void WriteAllBytes(string filename, byte[] encodedTextBytes)
        {
            System.IO.File.WriteAllBytes(filename,encodedTextBytes);
        }

        public bool CheckFilenameForQuickSave(string filename)
        {
            return true;
        }

        public bool Exists(string filename)
        {
            return System.IO.File.Exists(filename);
        }

        public string ReadAllText(string filename, Encoding selectedEncoding)
        {
            return System.IO.File.ReadAllText(filename,selectedEncoding);
        }

        public Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter, DialogDirection dialogDirection, string title = null)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadAllBytes(string filename)
        {
            return System.IO.File.ReadAllBytes(filename);
        }

        public string PathGetFileName(string filename)
        {
            return System.IO.Path.GetFileName(filename);
        }
    }
}
