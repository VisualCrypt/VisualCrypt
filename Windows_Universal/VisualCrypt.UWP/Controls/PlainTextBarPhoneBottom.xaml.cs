﻿using Windows.UI.Xaml;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.UWP.Controls
{
    public sealed partial class PlainTextBarPhoneBottom
    {
        readonly PortableMainViewModel _viewModel;

        public PlainTextBarPhoneBottom()
        {
            this.InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
        }

        async void Hyperlink_SetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ShowSetPasswordDialogCommand.CanExecute())
                await _viewModel.ShowSetPasswordDialogCommand.Execute();
        }

        async void Hyperlink_ClearPassword_MouseDown(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ClearPasswordCommand.CanExecute())
                await _viewModel.ClearPasswordCommand.Execute();
        }

        async void Hyperlink_Encrypt_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.EncryptEditorContentsCommand.CanExecute())
                await _viewModel.EncryptEditorContentsCommand.Execute();
        }

        async void Hyperlink_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.DecryptEditorContentsCommand.CanExecute())
                await _viewModel.DecryptEditorContentsCommand.Execute();
        }

        async void Hyperlink_Save_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SaveCommand.CanExecute())
                await _viewModel.SaveCommand.Execute();
        }
    }
}
