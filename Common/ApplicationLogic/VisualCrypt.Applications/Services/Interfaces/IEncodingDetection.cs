using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IEncodingDetection
    {
        Encoding PlatformDefaultEncoding { set; }

        Response<string, Encoding> GetStringFromFile(byte[] data, LongRunningOperationContext context);
     
    }
}
