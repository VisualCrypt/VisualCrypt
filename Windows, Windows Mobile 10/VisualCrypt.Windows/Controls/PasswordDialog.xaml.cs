using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class PasswordDialog : UserControl
    {


        readonly PasswordDialogViewModel _viewModel;
        public PasswordDialog(IEncryptionService encryptionService, SetPasswordDialogMode setPasswordDialogMode,
            Action<bool> closePopup, Action<bool> setIsPasswordSet, bool isPasswordSetWhenDialogOpened)
        {
            this.InitializeComponent();
            _viewModel = new PasswordDialogViewModel(encryptionService, setPasswordDialogMode, closePopup, setIsPasswordSet, isPasswordSetWhenDialogOpened);
        }



        void Hyperlink_CreatePassword_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            _viewModel.Hyperlink_CreatePassword_OnClick(sender, args);
        }
    }
}
