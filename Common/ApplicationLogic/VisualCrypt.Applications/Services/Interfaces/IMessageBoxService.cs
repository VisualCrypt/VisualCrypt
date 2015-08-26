using System;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IMessageBoxService
    {
        RequestResult Show(string messageBoxText, string title, RequestButton buttons,
            RequestImage image);

        void ShowError(Exception e, [CallerMemberName] string callerMemberName = "");

        void ShowError(string error);
    }
}
