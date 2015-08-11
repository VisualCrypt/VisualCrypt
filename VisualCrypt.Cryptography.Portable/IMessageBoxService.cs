using System;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Cryptography.Portable
{
    public interface IMessageBoxService
    {
        RequestResult Show(string messageBoxText, string title, RequestButton buttons,
            RequestImage image);

        void ShowError(Exception e, [CallerMemberName] string callerMemberName = "");

        void ShowError(string error);
    }

    public enum RequestResult
    {
        Cancel,
        OK,
        None
    }

    public enum RequestImage
    {
        Warning,
        Exclamation,
        Information,
        Error,
        Question
    }

    public enum RequestButton
    {
        OK,Cancel,
        OKCancel
    }
}
