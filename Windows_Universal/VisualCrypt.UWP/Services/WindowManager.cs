using System;
using System.Text;
using System.Threading.Tasks;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.UWP.Services
{
    class WindowManager : IWindowManager
    {
        public Task<bool> GetBoolFromShowDialogAsyncWhenClosed<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task GetBoolFromShowDialogAsyncWhenClosed_SettingsDialog()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetDialogFromShowDialogAsyncWhenClosed<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<bool?, Encoding>> GetDialogFromShowDialogAsyncWhenClosed_ImportEncodingDialog()
        {
            throw new NotImplementedException();
        }

        public Task ShowAboutDialogAsync()
        {
            throw new NotImplementedException();
        }

        public Task ShowLogWindowAsync()
        {
            throw new NotImplementedException();
        }

        public Task<object> ShowWindowAsyncAndWaitForClose<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}