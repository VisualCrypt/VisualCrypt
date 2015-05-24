using System;
using System.Reflection;
using System.Windows;
using VisualCrypt.Portable.Tools;

namespace VisualCrypt.Desktop.Views
{
    class MessageBoxService : IMessageBoxService
    {
        readonly Window _owner;

        public MessageBoxService() { }

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
                throw new ArgumentNullException("title");

            try
            {
                if (_owner != null)
                    return MessageBox.Show(_owner, messageBoxText, title, buttons, image,MessageBoxResult.Cancel);
                return MessageBox.Show(messageBoxText, title, buttons, image);
            }
            catch (Exception e)
            {
                return MessageBox.Show("{0}\r\n\r\n{1}".FormatInvariant(e.Message, messageBoxText), title, buttons, image);
            }
        }


        public void ShowError(MethodBase methodBase, Exception e)
        {
            var methodName = "Program";
            if (methodBase != null)
                methodName = methodBase.Name;

            var messageBoxText = "Error in {0}:\r\n\r\n{1}".FormatInvariant(methodName, e.Message);
            Show(messageBoxText, "Error (Press Ctrl + C to copy)", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public void ShowError(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                error = "There was an error but the error message is missing.";

            
            Show(error, "Error (Press Ctrl + C to copy)", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
