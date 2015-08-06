using System.Collections.Generic;
using System.Threading;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.App.API.V2;
using VisualCrypt.App.Common;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.AppLogic;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure;


namespace VisualCrypt.App
{
    public sealed partial class EditorPage : IFileSavePickerContinuable
    {
        readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        readonly IVisualCrypt2API _visualCryptAPI;
        readonly ICoreAPI2 _visualCryptCoreAPI;

        public EditorPage()
        {
            InitializeComponent();

            _visualCryptCoreAPI = new Core2API();
            _visualCryptAPI = new VisualCrypt2API(_visualCryptCoreAPI);

            Loaded += (sender, e) => HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Unloaded += (sender, e) => HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TextBlockTitle.Text = ModelState.FileName;
            TextBox1.Text = ModelState.TextBuffer;

            if (e.Parameter is SetPasswordDialogMode)
            {
                var setPasswordDialogMode = (SetPasswordDialogMode)e.Parameter;
                switch (setPasswordDialogMode)
                {
                    case SetPasswordDialogMode.Set:
                        break;
                    case SetPasswordDialogMode.SetAndEncrypt:
                        Encrypt();
                        break;
                    case SetPasswordDialogMode.SetAndDecrypt:
                        Decrypt();
                        break;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ModelState.TextBuffer = TextBox1.Text;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            var dialog = new MessageDialog("Discard Changes?");
            dialog.Commands.Add(new UICommand("Cancel", _=> { ; }));
            dialog.Commands.Add(new UICommand("Ok", _ => Frame.Navigate(typeof(HubPage))));
            dialog.ShowAsync();
        }

        void ButtonEncrypt_Click(object sender, RoutedEventArgs e)
        {
			Encrypt();
			return;
            if (string.IsNullOrWhiteSpace(ModelState.Password))
                Frame.Navigate(typeof(PasswordPage), SetPasswordDialogMode.SetAndEncrypt);
            else
            {
                Encrypt();
            }
        }

        void ButtonDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ModelState.Password))
                    Frame.Navigate(typeof(PasswordPage), SetPasswordDialogMode.SetAndDecrypt);
                else
                {
                    Decrypt();
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                dialog.ShowAsync();
            }
        }

        void ButtonPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(PasswordPage), SetPasswordDialogMode.Set))
            {
                throw new Exception(_resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        async void ButtonEmail_OnClick(object sender, RoutedEventArgs e)
        {
            var mail = new EmailMessage();
            mail.Subject = "VisualCrypt Message";
            mail.Body = TextBox1.Text;
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }


        void ButtonClear_OnClick(object sender, RoutedEventArgs e)
        {
            ModelState.TextBuffer = string.Empty;
            TextBox1.Text = ModelState.TextBuffer;
        }

        void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
           
            TextBlockTitle.Text =  "";

            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            savePicker.PickSaveFileAndContinue();
        }

	    FileModel _fileModel = FileModel.EmptyCleartext();
        void Encrypt()
        {
	        var svc = new EncryptionService();

	        svc.SetPassword("bla");
	        var response = svc.EncryptForDisplay(_fileModel, TextBox1.Text,
		        new LongRunningOperationContext(new CancellationToken(), new EncryptionProgress(delegate { })));

	        _fileModel = response.Result;
	        TextBox1.Text = _fileModel.VisualCryptText;
        }

        void Decrypt()
        {
			var svc = new EncryptionService();
			svc.SetPassword("bla");

			var response = svc.DecryptForDisplay(_fileModel,TextBox1.Text, new LongRunningOperationContext(new CancellationToken(), new EncryptionProgress(delegate { })));
	        _fileModel = response.Result;
	        TextBox1.Text = _fileModel.ClearTextContents;
        }







        public async void ContinueFileSavePicker(Windows.ApplicationModel.Activation.FileSavePickerContinuationEventArgs args)
        {
            StorageFile file = args.File;
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, TextBox1.Text);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    TextBlockTitle.Text = file.Name + " was saved.";
                }
                else
                {
                    TextBlockTitle.Text = file.Name + " couldn't be saved.";
                }
            }
            else
            {
                TextBlockTitle.Text = "Operation cancelled.";
            }
        }
    }
}