using System;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Windows.Services
{
    interface IMessageBoxService
    {
        MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons,
            MessageBoxImage image);

        void ShowError(Exception e, [CallerMemberName] string callerMemberName = "");

        void ShowError(string error);
    }

    enum MessageBoxResult
    {
        Cancel,
        OK
    }

    enum MessageBoxImage
    {
        Warning
    }

    enum MessageBoxButton
    {
        OKCancel
    }
}
