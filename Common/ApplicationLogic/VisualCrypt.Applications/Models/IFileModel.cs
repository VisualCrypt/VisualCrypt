using System.Text;

namespace VisualCrypt.Applications.Models
{
    public interface IFileModel
    {
        bool IsDirty { get; set; }
        bool IsEncrypted { get;  }
        Encoding SaveEncoding  { get;  }
	  
    }
}
