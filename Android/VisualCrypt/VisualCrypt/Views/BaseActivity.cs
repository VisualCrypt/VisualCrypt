using Android.App;
using Android.Support.V7.App;
using VisualCrypt.Language.Strings;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;

namespace VisualCrypt.Views
{
    [Activity(Label = "VisualCrypt")]
    public class BaseActivity : AppCompatActivity // http://developer.android.com/reference/android/support/v7/app/AppCompatActivity.html
    {
        public static BaseActivity LastResumedActivityInstance { get; private set; }
        static bool isInitialized;

        protected readonly ResourceWrapper _resourceWrapper;
       
        public BaseActivity()
        {
            if(!isInitialized)
            {
                Bootstrapper.Register();
                isInitialized = true;
            }
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }

        protected override void OnResume()
        {
            base.OnStart();
            LastResumedActivityInstance = this;
        }
    }
}