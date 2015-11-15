using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IFileService
    {
        bool CheckFilenameForQuickSave(string filename);
        bool Exists(string filename);
        string PathGetFileName(string filename);
        Task<Tuple<bool, string>> PickFileAsync(string suggestedFilename, DialogFilter diaglogFilter, FileDialogMode fileDialogMode, string title);
        byte[] ReadAllBytes(string filename);
        string ReadAllText(string filename, Encoding selectedEncoding);
        void WriteAllBytes(string filename, byte[] encodedTextBytes);
        string GetEncodingDisplayString(Encoding saveEncoding);
        Task<ObservableCollection<FileReference>> GetFileReferences(string directoryPath);
        Task DeleteAsync(string pathAndFilename);
    }
}
