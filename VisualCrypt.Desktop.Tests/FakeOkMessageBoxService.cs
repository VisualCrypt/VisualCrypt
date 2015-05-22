using System.Windows;
using VisualCrypt.Desktop.Views;

namespace VisualCrypt.Desktop.Tests
{
    class FakeOkMessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons, MessageBoxImage image)
        {
            return MessageBoxResult.OK;
        }


        public void ShowError(System.Reflection.MethodBase methodBase, System.Exception e)
        {
            
        }
    }
}
