using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IMessageBoxService
    {
        Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons,
            RequestImage image);

        Task ShowError(Exception e, [CallerMemberName] string callerMemberName = "");

        Task ShowError(string error);
    }
}
