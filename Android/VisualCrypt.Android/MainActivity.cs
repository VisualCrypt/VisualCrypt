using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using VisualCrypt.Cryptography.Net;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;

namespace VisualCrypt.Droid
{
    [Activity(Label = "VisualCrypt", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Register();

            // Get our UI controls from the loaded layout:
            EditText phoneNumberText = FindViewById<EditText>(Resource.Id.PhoneNumberText);
            Button translateButton = FindViewById<Button>(Resource.Id.buttonTranslate);
            Button callButton = FindViewById<Button>(Resource.Id.buttonCall);

            // Disable the "Call" button
            callButton.Enabled = false;

            // Add code to translate number
            string translatedNumber = string.Empty;

            translateButton.Click += (object sender, EventArgs e) =>
            {
                // Translate user's alphanumeric phone number to numeric
                translatedNumber = PhoneTranslator.ToNumber(phoneNumberText.Text);
                if (String.IsNullOrWhiteSpace(translatedNumber))
                {
                    callButton.Text = "Call";
                    callButton.Enabled = false;
                }
                else
                {
                    callButton.Text = "Call " + translatedNumber;
                    callButton.Enabled = true;
                }
            };

            callButton.Click += (object sender, EventArgs e) =>
            {
                // On "Call" button click, try to dial phone number.
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Call " + translatedNumber + "?");
                callDialog.SetNeutralButton("Call", delegate {
                    // Create intent to dial phone
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
                    StartActivity(callIntent);
                });
                callDialog.SetNegativeButton("Cancel", delegate { });

                // Show the alert dialog to the user and wait for response.
                callDialog.Show();
            };

            // Basic configuration test
            var api = Service.Get<IVisualCrypt2Service>();
            var response = api.SuggestRandomPassword();
            if (response.IsSuccess)
                phoneNumberText.Text = response.Result;
            else
                phoneNumberText.Text = "Error";

        }

        void Register()
        {
            // Bootstrap the VisualCrypt API
            Service.Register<IPlatform, Platform_Net4>(true);
            Service.Register<IVisualCrypt2Service, VisualCrypt2Service>(true);
            Service.Get<IVisualCrypt2Service>().Platform = Service.Get<IPlatform>();
        }
    }
}

