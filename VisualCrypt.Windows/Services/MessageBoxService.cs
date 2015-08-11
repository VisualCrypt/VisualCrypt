using System;
using Windows.UI.Popups;

namespace VisualCrypt.Windows.Services
{
    class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons, MessageBoxImage image)
        {

            ShowDialog(messageBoxText);
          
           return MessageBoxResult.OK;
          
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
