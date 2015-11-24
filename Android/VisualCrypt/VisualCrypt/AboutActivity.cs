using Android.App;
using Android.OS;
using Android.Support.V7.App;
using VisualCrypt.Language.Strings;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;

namespace VisualCrypt
{
    [Activity(Label = "", ParentActivity = typeof(FilesActivity))]
    public class AboutActivity : AppCompatActivity
    {
        readonly ResourceWrapper _resourceWrapper;
        public AboutActivity()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.About);
            var toolbar = FindViewById<Toolbar>(Resource.Id.about_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = _resourceWrapper.miHelpAbout.NoDots();
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
    }
}