using System;
using System.ComponentModel.Composition;
using System.Windows;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.Tests
{
	[Export(typeof(IMessageBoxService))]
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