using System.Text;

namespace VisualCrypt.Cryptography.Portable.Apps.Models
{
    public interface IFileModel
    {
        bool IsDirty { get; set; }
        bool IsEncrypted { get;  }
        Encoding SaveEncoding  { get;  }
	  
    }
}
