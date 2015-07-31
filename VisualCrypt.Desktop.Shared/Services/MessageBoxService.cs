using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Windows;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Desktop.Shared.Services
{
	[Export(typeof (IMessageBoxService))]
	public class MessageBoxService : IMessageBoxService
	{
		readonly Window _owner;

		Window Owner
		{
			get
			{
				if (_owner != null)
					return _owner;
				return Application.Current.MainWindow;
			}
		}

		/// <summary>
		/// The owner Window will be Application.Current.MainWindow (lazy getter). If there is no MainWindow,
		/// MessageBox.Show() without specifying an owner will be used.
		/// </summary>
		public MessageBoxService()
		{
		}

		/// <summary>
		/// As supplied by MEF or if the default c'tor is used, the owner Window 
		/// will always be Application.Current.MainWindow (lazy getter).
		/// Only use this c'tor, if another window shall be the owner.
		/// </summary>
		public MessageBoxService(Window owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			_owner = owner;
		}

		/// <summary>
		/// Defaults to the Cancel button, if present.
		/// </summary>
		public MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton buttons, MessageBoxImage image)
		{
			if (title == null)
				title = "Untitled";
			if (messageBoxText == null)
				messageBoxText = "No text available";

			try
			{
				if (Owner != null)
					return MessageBox.Show(Owner, messageBoxText, title, buttons, image, MessageBoxResult.Cancel);
				return MessageBox.Show(messageBoxText, title, buttons, image);
			}
			catch (Exception e)
			{
				return MessageBox.Show("{0}\r\n\r\n{1}".FormatInvariant(e.Message, messageBoxText), title, buttons, image);
			}
		}


		public void ShowError(Exception e, [CallerMemberName] string callerMemberName = "")
		{
			var messageBoxText = "SetError in {0}:\r\n\r\n{1}".FormatInvariant(callerMemberName, e.Message);
			Show(messageBoxText, "SetError (Press Ctrl + C to copy)", MessageBoxButton.OK, MessageBoxImage.Error);
		}


		public void ShowError(string error)
		{
			if (string.IsNullOrWhiteSpace(error))
				error = "There was an error but the error message is missing.";


			Show(error, "SetError (Press Ctrl + C to copy)", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}