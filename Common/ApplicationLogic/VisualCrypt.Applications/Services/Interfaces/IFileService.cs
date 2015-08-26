using System;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IFileService
    {
        void WriteAllBytes(string filename, byte[] encodedTextBytes);

        bool CheckFilenameForQuickSave(string filename);

	    bool Exists(string filename);
	    string ReadAllText(string filename, Encoding selectedEncoding);

	    Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter,
		    DialogDirection dialogDirection, string title = null);

        /// <summary>
        /// File.ReadAllBytes(string filename)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        byte[] ReadAllBytes(string filename);

        /// <summary>
        /// Path.GetFileName(string filename)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        string PathGetFileName(string filename);
    }
}
