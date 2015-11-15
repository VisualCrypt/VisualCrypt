using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.UWP.Services
{
    class MessageBoxService : IMessageBoxService
    {
        readonly ResourceWrapper _resourceWrapper;
        RequestResult _requestResult;

        public MessageBoxService()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }
        public async Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {

            var commands = ButtonToCommands(buttons);

            var md = new MessageDialog(messageBoxText, title);
            foreach (var c in commands)
                md.Commands.Add(c);

            await md.ShowAsync();

            return _requestResult;
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

        List<UICommand> ButtonToCommands(RequestButton buttons)
        {
            var commands = new List<UICommand>();
            switch (buttons)
            {
                case RequestButton.OK:
                    {
                        var okBtn = new UICommand(_resourceWrapper.termOK)
                        {
                            Invoked = e => { _requestResult = RequestResult.OK; }
                        };
                        commands.Add(okBtn);
                    }
                    break;
                case RequestButton.OKCancel:
                    {
                        var okBtn = new UICommand(_resourceWrapper.termOK)
                        {
                            Invoked = e => { _requestResult = RequestResult.OK; }
                        };
                        commands.Add(okBtn);

                        var cancelButton = new UICommand(_resourceWrapper.termCancel)
                        {
                            Invoked = e => { _requestResult = RequestResult.Cancel; }
                        };
                        commands.Add(cancelButton);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
            return commands;
        }

    }
}
