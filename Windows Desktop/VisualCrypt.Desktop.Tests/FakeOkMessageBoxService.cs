using System;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Tests
{
	internal class FakeOkMessageBoxService : IMessageBoxService
	{
		public void ShowError(Exception e, string callerMemberName = "callerMemberName")
		{
		}


		public void ShowError(string error)
		{
		}

        public RequestResult Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {
            return RequestResult.OK;
        }
    }
}