using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace VisualCrypt.Desktop.Shared.Services
{
	public interface IMessageBoxService
	{
		MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons,
			MessageBoxImage image);

		void ShowError(Exception e, [CallerMemberName] string callerMemberName = "");

		void ShowError(string error);
	}
}