using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;

namespace VisualCrypt.UWP.Controls
{
    public sealed partial class PasswordUserControl : UserControl
    {
        PortablePasswordDialogViewModel _viewModel;
        public PasswordUserControl()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortablePasswordDialogViewModel>();
        }

        public void InitViewModel(IEncryptionService encryptionService, SetPasswordDialogMode setPasswordDialogMode,
            Action<bool> setIsPasswordSet, Action<bool> closeAction, bool isPasswordSetWhenDialogOpened)
        {
            _viewModel.Init(setPasswordDialogMode, setIsPasswordSet, closeAction, isPasswordSetWhenDialogOpened);
            PasswordBox.Focus(FocusState.Programmatic);
        }
      

        async void Hyperlink_SuggestPassword_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            PasswordBox.Focus(FocusState.Programmatic);
            if (_viewModel.SuggestPasswordCommand.CanExecute())
                await _viewModel.SuggestPasswordCommand.Execute();
        }

        async void Hyperlink_Print_Password_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            PasswordBox.Focus(FocusState.Programmatic);
            if (_viewModel.PrintPasswordCommand.CanExecute())
                await _viewModel.PrintPasswordCommand.Execute();
        }

        void Hyperlink_Spec_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            PasswordBox.Focus(FocusState.Programmatic);
        }
    }
}
