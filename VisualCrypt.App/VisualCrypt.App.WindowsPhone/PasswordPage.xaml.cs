using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;


namespace VisualCrypt.App
{
    public sealed partial class PasswordPage
    {
        //readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        SetPasswordDialogMode _setPasswordDialogMode;

        public PasswordPage()
        {
            InitializeComponent();

            Loaded += (sender, e) => HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Unloaded += (sender, e) => HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            var dialog = new MessageDialog("Discard Changes?");
            dialog.Commands.Add(new UICommand("Cancel", _ => { }));
            dialog.Commands.Add(new UICommand("Ok", _ => Frame.Navigate(typeof(EditorPage))));
            dialog.ShowAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TextBoxPassword.Text = ModelState.Password;

            if (e.Parameter is SetPasswordDialogMode)
            {
                _setPasswordDialogMode = (SetPasswordDialogMode)e.Parameter;
            }
            else
            {
                // we are resuming
                _setPasswordDialogMode = SetPasswordDialogMode.Set;
            }

            switch (_setPasswordDialogMode)
            {
                case SetPasswordDialogMode.Set:
                    TextBlockTitle.Text = "Set Password";
                    ButtonSetPassword.Content = "Set";
                    break;
                case SetPasswordDialogMode.SetAndEncrypt:
                    TextBlockTitle.Text = "Set Password & Encrypt";
                    ButtonSetPassword.Content = "Encrypt";
                    break;
                case SetPasswordDialogMode.SetAndDecrypt:
                    TextBlockTitle.Text = "Set Password & Decrpyt";
                    ButtonSetPassword.Content = "Decrypt";
                    break;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

        }

        void ButtonSetPassword_OnClick(object sender, RoutedEventArgs e)
        {
            ModelState.Password = TextBoxPassword.Text;
            Frame.Navigate(typeof(EditorPage), _setPasswordDialogMode);
        }
    }
}