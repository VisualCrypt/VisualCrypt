using System.Text;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Applications.Apps.Services
{
    public interface IEncodingDetection
    {
        Response<string, Encoding> GetStringFromFile(byte[] data, Encoding platformDefaultEncoding);
    }
}
