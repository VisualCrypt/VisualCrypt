using System;
using System.ComponentModel.Composition;
using System.Windows;
using VisualCrypt.Desktop.Shared.Services;

namespace VisualCrypt.Desktop.Tests
{
	[Export(typeof(IMessageBoxService))]
	internal class FakeOkMessageBoxService : IMessageBoxService
	{
		public MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons, MessageBoxImage image)
		{
			return MessageBoxResult.OK;
		}

		public void ShowError(Exception e, string callerMemberName = "callerMemberName")
		{
		}


		public void ShowError(string error)
		{
		}
	}
}