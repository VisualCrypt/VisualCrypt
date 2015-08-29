using Android.App;
using Android.Widget;
using Android.OS;
using VisualCrypt.Cryptography.Net;
using VisualCrypt.Cryptography.VisualCrypt2.Implementations;
using VisualCrypt.Cryptography.VisualCrypt2.Interfaces;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;

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
            Register();

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            // Basic configuration test
            var api = Service.Get<IVisualCrypt2Service>();
            var response = api.SuggestRandomPassword();
            if (response.IsSuccess)
                button.Text = "OK";
            else
                button.Text = "Error";

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

