using System;
using Windows.UI.Popups;
using VisualCrypt.Applications.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VisualCrypt.Windows.Services
{
    class MessageBoxService : IMessageBoxService
    {
        RequestResult _requestResult;
        public async Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {
            var tcs = new TaskCompletionSource<RequestResult>();
            var commands = ButtonToCommands(buttons, tcs);

            var md = new MessageDialog(messageBoxText, title);
            foreach (var c in commands)
                md.Commands.Add(c);

            await md.ShowAsync();

            return _requestResult;
        }

        List<UICommand> ButtonToCommands(RequestButton buttons, TaskCompletionSource<RequestResult> tcs)
        {
            var commands = new List<UICommand>();
            if (buttons == RequestButton.OK)
            {
                UICommand okBtn = new UICommand("OK");
                okBtn.Invoked = (e) => { _requestResult = RequestResult.OK; };
                commands.Add(okBtn);
            }
            if (buttons == RequestButton.OKCancel)
            {
                //OK Button
                UICommand okBtn = new UICommand("OK");
                okBtn.Invoked = (e) => { _requestResult = RequestResult.OK; };
                commands.Add(okBtn);

                //OK Button
                UICommand cancelButton = new UICommand("Cancel");
                cancelButton.Invoked = (e) => { _requestResult = RequestResult.Cancel; };
                commands.Add(cancelButton);
            }
            else throw new NotImplementedException();
            return commands;
        }

        public async Task ShowError(Exception e, string callerMemberName = "")
        {
            await ShowDialog(e.Message);
        }

        public async Task ShowError(string error)
        {
            await ShowDialog(error);
        }

        async Task ShowDialog(string text)
        {
            var md = new MessageDialog(text, "Error");
            await md.ShowAsync();
        }


    }
}
