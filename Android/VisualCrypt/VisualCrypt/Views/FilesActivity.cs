using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.temp;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace VisualCrypt.Views
{
    [Activity(MainLauncher = true, Icon = "@mipmap/icon")]
    public class FilesActivity : BaseActivity
    {
        public static FilesActivity Instance { get; private set; }

        ListView _listView_files;
        IList<FileReference> _fileReferences;
        FileReferencesAdapter _fileReferencesAdapter;
        PortableFilesPageViewModel _viewModel;
        IFileService _fileService;
        public FilesActivity()
        {
            _viewModel = Service.Get<PortableFilesPageViewModel>();
            _fileService = Service.Get<IFileService>();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Files);
            var toolbar = FindViewById<Toolbar>(Resource.Id.files_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = _resourceWrapper.termNotes;

            _listView_files = FindViewById<ListView>(Resource.Id.listView_files);
            _listView_files.ItemClick += async (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                await _viewModel.NavigateToOpenCommand.Execute(_fileReferences[e.Position]);
                //var taskDetails = new Intent(this, typeof(MainActivity));
                //taskDetails.PutExtra("TaskID", _fileReferences[e.Position].PathAndFileName);
                //StartActivity(taskDetails);
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            Instance = this;
            var task = _fileService.GetFileReferences(null);
            task.Wait();

            _fileReferences = task.Result;
           
            //_fileReferences = FileDatabase.GetItems();

            // create our adapter
            _fileReferencesAdapter = new FileReferencesAdapter(this, _fileReferences);

            //Hook up our adapter to our ListView
            _listView_files.Adapter = _fileReferencesAdapter;
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
                    if (_viewModel.NavigateToNewCommand.CanExecute())
                        _viewModel.NavigateToNewCommand.Execute().Wait();
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