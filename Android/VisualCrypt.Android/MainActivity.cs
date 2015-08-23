using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using VisualCrypt.Cryptography.Net.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;

namespace VisualCrypt.Android
{
    [Activity(Label = "VisualCrypt.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            var api = new VisualCrypt2API(new Platform_Net4());
            button.Text = api.GenerateRandomPassword().Result;
        }
    }
}

