using Windows.ApplicationModel.DataTransfer;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.UWP.Services
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