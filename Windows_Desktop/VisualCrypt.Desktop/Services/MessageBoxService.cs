﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using VisualCrypt.Applications.Extensions;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Services
{
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

        public async Task ShowError(Exception e, [CallerMemberName] string callerMemberName = "")
        {
            var tcs = new TaskCompletionSource<RequestResult>();
            
            ShowAsync(e.Message, "VisualCrypt", RequestButton.OK, RequestImage.Error, tcs.SetResult);

            await tcs.Task;
        }


        public async Task ShowError(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                error = "Unspecified Error";

            var tcs = new TaskCompletionSource<RequestResult>();

            ShowAsync(error, "VisualCrypt", RequestButton.OK, RequestImage.Error,tcs.SetResult);

            await tcs.Task;

        }

        public async Task<RequestResult> Show(string messageBoxText, string title, RequestButton buttons, RequestImage image)
        {
            var tcs = new TaskCompletionSource<RequestResult>();

            ShowAsync(messageBoxText, title, buttons, image, tcs.SetResult);

            return await tcs.Task;
        }

        void ShowAsync(string messageBoxText, string title, RequestButton buttons, RequestImage image, Action<RequestResult> callback)
        {
            if (title == null)
                title = "VisualCrypt";
            if (messageBoxText == null)
                messageBoxText = "No text available";

            var xButtons = (MessageBoxButton)buttons;
            var xImages = (MessageBoxImage)image;

            RequestResult result;

            try
            {

                if (Owner != null)
                    result = (RequestResult)MessageBox.Show(Owner, messageBoxText, title, xButtons, xImages, MessageBoxResult.Cancel);
                else
                    result = (RequestResult)MessageBox.Show(messageBoxText, title, xButtons, xImages);
            }
            catch (Exception e)
            {
                result = (RequestResult)MessageBox.Show("{0}\r\n\r\n{1}".FormatInvariant(e.Message, messageBoxText), title, xButtons, xImages);
            }
            callback(result);
        }
    }
}