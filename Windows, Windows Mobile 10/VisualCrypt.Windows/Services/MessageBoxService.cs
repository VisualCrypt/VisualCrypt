using System;
using Windows.UI.Popups;
using VisualCrypt.Applications.Apps.Services;

namespace VisualCrypt.Windows.Services
{
    class MessageBoxService : IMessageBoxService
    {
        public RequestResult Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {

            ShowDialog(messageBoxText);
          
           return RequestResult.OK;
          
        }

        public void ShowError(Exception e, string callerMemberName = "")
        {
            ShowDialog(e.Message);
        }

        public void ShowError(string error)
        {
            ShowDialog(error);
        }

        async void ShowDialog(string text)
        {
            var md = new MessageDialog(text);
            await md.ShowAsync();
        }
    }
}
