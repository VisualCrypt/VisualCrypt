using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Tests
{
	internal class FakeOkMessageBoxService : IMessageBoxService
	{

        async Task<RequestResult> IMessageBoxService.Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {
            Trace.Write(messageBoxText);
          
            var tcs = new TaskCompletionSource<RequestResult>();
            tcs.SetResult(RequestResult.OK);
            return await tcs.Task;
        }

        async Task IMessageBoxService.ShowError(Exception e, string callerMemberName)
        {
            Trace.Write(e);
            var tcs = new TaskCompletionSource<RequestResult>();
            tcs.SetResult(RequestResult.OK);
            await tcs.Task;
        }

        async Task IMessageBoxService.ShowError(string error)
        {
            Trace.Write(error);
            var tcs = new TaskCompletionSource<RequestResult>();
            tcs.SetResult(RequestResult.OK);
           await tcs.Task;
        }
    }
}