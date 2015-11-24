using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using VisualCrypt.Language.Strings;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;

namespace VisualCrypt
{
    [Activity(Label = "VisualCrypt", MainLauncher = true, Icon = "@mipmap/icon")]
    public class FilesActivity : AppCompatActivity
    {
        static bool isInitialized;
        readonly ResourceWrapper _resourceWrapper;
        public FilesActivity()
        {
            if(!isInitialized)
            {
                Bootstrapper.Register();
                isInitialized = true;
            }
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Files);
            var toolbar = FindViewById<Toolbar>(Resource.Id.files_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = _resourceWrapper.termNotes;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_files, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_new:
                    StartActivity(typeof(MainActivity));
                    return true;
                case Resource.Id.action_open:
                    ;
                    return true;
                case Resource.Id.action_select:
                    ;
                    return true;
                case Resource.Id.action_settings:
                    StartActivity(typeof(SettingsActivity));
                    return true;
                case Resource.Id.action_about:
                    StartActivity(typeof(AboutActivity));
                    return true;
                case Resource.Id.action_cancelSelect:
                    ;
                    return true;
                case Resource.Id.action_rename:
                    ;
                    return true;
                case Resource.Id.action_delete:
                    ;
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}