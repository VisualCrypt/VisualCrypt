using VisualCrypt.Applications.Services.Interfaces;
using Windows.ApplicationModel.DataTransfer;

namespace VisualCrypt.Windows.Services
{
    public class ClipBoardService : IClipBoardService
    {
        public void CopyText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            DataPackage dataPackage = new DataPackage {  RequestedOperation = DataPackageOperation.Copy};
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        // see https://msdn.microsoft.com/en-us/library/windows/apps/mt243291.aspx
        // on how to monitor clipboard.
    }
}