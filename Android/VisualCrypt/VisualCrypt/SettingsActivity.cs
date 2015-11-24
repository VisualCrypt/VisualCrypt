using Android.App;
using Android.OS;
using Android.Support.V7.App;
using VisualCrypt.Language.Strings;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;

namespace VisualCrypt
{
    [Activity(Label = "", ParentActivity = typeof(FilesActivity))]
    public class SettingsActivity : AppCompatActivity
    {
        readonly ResourceWrapper _resourceWrapper;
        public SettingsActivity()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);
            var toolbar = FindViewById<Toolbar>(Resource.Id.settings_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = _resourceWrapper.miVCSettings.NoDots();
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
    }
}