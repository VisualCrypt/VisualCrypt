using Android.App;
using Android.OS;
using VisualCrypt.Language.Strings;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace VisualCrypt.Views
{
    [Activity(ParentActivity = typeof(FilesActivity))]
    public class SettingsActivity : BaseActivity
    {
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