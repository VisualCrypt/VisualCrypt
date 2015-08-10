using System.Text;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic
{
    public interface IFileModel
    {
        bool IsDirty { get; set; }
        bool IsEncrypted { get;  }
        Encoding SaveEncoding  { get;  }
    }
}
