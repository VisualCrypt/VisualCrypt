using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Views;

namespace VisualCrypt.Droid
{
    class MessageBoxService : IMessageBoxService
    {
        public async Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {

            var tcs = new TaskCompletionSource<RequestResult>();
            var builder = new AlertDialog.Builder(BaseActivity.LastResumedActivityInstance);
            
            builder.SetTitle(title)
                   .SetMessage(messageBoxText)
                   .SetPositiveButton("Yes", delegate { tcs.SetResult(RequestResult.OK); })
                   .SetNegativeButton("No", delegate { tcs.SetResult(RequestResult.Cancel); });

            builder.Create().Show();
            return await tcs.Task;

        }

        public Task ShowError(string error)
        {
            return Show(error, "VisualCrypt", RequestButton.OK, RequestImage.Error);
        }

        public Task ShowError(Exception e, [CallerMemberName] string callerMemberName = "")
        {
            return Show(e.Message, "VisualCrypt", RequestButton.OK, RequestImage.Error);
        }
    }
}