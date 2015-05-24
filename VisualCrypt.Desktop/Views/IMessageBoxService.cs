using System;
using System.Reflection;
using System.Windows;

namespace VisualCrypt.Desktop.Views
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons,
            MessageBoxImage image);

       

        void ShowError(MethodBase methodBase, Exception e);

        void ShowError(string error);
    }
}
