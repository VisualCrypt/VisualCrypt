using System;

namespace VisualCrypt.Windows.Services
{
    class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            return MessageBoxResult.OK;
        }

        public void ShowError(Exception e, string callerMemberName = "")
        {
           
        }

        public void ShowError(string error)
        {
            
        }
    }
}
