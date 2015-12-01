using Android.App;
using Android.OS;
using VisualCrypt.Language.Strings;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace VisualCrypt.Views
{
    [Activity(ParentActivity = typeof(FilesActivity))]
    public class AboutActivity : BaseActivity
    {
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