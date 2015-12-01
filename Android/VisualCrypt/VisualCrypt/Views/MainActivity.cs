using Android.App;
using Android.OS;
using Android.Support.V7.App;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Service = VisualCrypt.Applications.Services.Interfaces.Service;


using System;
using Android.Views;
using VisualCrypt.Applications.ViewModels;
using Android.Widget;
using VisualCrypt.Applications.Constants;
using VisualCrypt.Applications.Services.Interfaces;
using System.Threading.Tasks;
using VisualCrypt.Applications.Models;

namespace VisualCrypt.Views
{
    [Activity(ParentActivity = typeof(FilesActivity))]
    public class MainActivity : BaseActivity
    {
        public static MainActivity MainActivityInstance { get; private set; }

        readonly PortableMainViewModel _mainViewModel;
        readonly PortableEditorViewModel _editorViewModel;
        readonly PlaintextBarFragment _plaintextBarFragment;
        readonly WorkingBarFragment _workingBarFragment;
        readonly EncryptedBarFragment _encryptedBarFragment;

        readonly PlaintextBarBottomFragment _plaintextBarBottomFragment;
        readonly WorkingBarBottomFragment _workingBarBottomFragment;
        readonly EncryptedBarBottomFragment _encryptedBarBottomFragment;
        readonly FilenameFragment _filenameFragment;
        readonly PasswordFragment _passwordFragment;

        public MainActivity()
        {
            _mainViewModel = Service.Get<PortableMainViewModel>();
            _editorViewModel = Service.Get<PortableEditorViewModel>();
            _plaintextBarFragment = new PlaintextBarFragment();
            _workingBarFragment = new WorkingBarFragment();
            _encryptedBarFragment = new EncryptedBarFragment();
            _plaintextBarBottomFragment = new PlaintextBarBottomFragment();
            _workingBarBottomFragment = new WorkingBarBottomFragment();
            _encryptedBarBottomFragment = new EncryptedBarBottomFragment();
            _filenameFragment = new FilenameFragment();
            _passwordFragment = new PasswordFragment();
        }

        private void StatusBarModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case nameof(_mainViewModel.StatusBarModel.IsPlaintextBarVisible):
                    if (_mainViewModel.StatusBarModel.IsPlaintextBarVisible)
                        SupportFragmentManager.BeginTransaction()
                            .Show(_plaintextBarFragment)
                            .Show(_plaintextBarBottomFragment)
                            .Hide(_workingBarFragment)
                            .Hide(_workingBarBottomFragment)
                            .Hide(_encryptedBarFragment)
                            .Hide(_encryptedBarBottomFragment)
                            .Commit();
                    break;
                case nameof(_mainViewModel.StatusBarModel.IsProgressBarVisible):
                    if (_mainViewModel.StatusBarModel.IsProgressBarVisible)
                        SupportFragmentManager.BeginTransaction()
                            .Hide(_plaintextBarFragment)
                            .Hide(_plaintextBarBottomFragment)
                            .Show(_workingBarFragment)
                            .Show(_workingBarBottomFragment)
                            .Hide(_encryptedBarFragment)
                            .Hide(_encryptedBarBottomFragment)
                            .Commit();
                    break;
                case nameof(_mainViewModel.StatusBarModel.IsEncryptedBarVisible):
                    if (_mainViewModel.StatusBarModel.IsEncryptedBarVisible)
                        SupportFragmentManager.BeginTransaction()
                            .Hide(_plaintextBarFragment)
                            .Hide(_plaintextBarBottomFragment)
                            .Hide(_workingBarFragment)
                            .Hide(_workingBarBottomFragment)
                            .Show(_encryptedBarFragment)
                            .Show(_encryptedBarBottomFragment)
                            .Commit();
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Console.WriteLine("MainActivity.OnCreate");

            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.main_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            Service.Get<ITextBoxController>(TextBoxName.TextBox1).PlatformTextBox = FindViewById<EditText>(Resource.Id.textBox1);
            _editorViewModel.OnViewLoaded();

            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.extendedAppBar_frameLayout, _plaintextBarFragment).Show(_plaintextBarFragment)
                .Add(Resource.Id.bottomBar_frameLayout, _plaintextBarBottomFragment).Show(_plaintextBarBottomFragment)
                .Add(Resource.Id.extendedAppBar_frameLayout, _workingBarFragment).Hide(_workingBarFragment)
                .Add(Resource.Id.bottomBar_frameLayout, _workingBarBottomFragment).Show(_workingBarBottomFragment)
                .Add(Resource.Id.extendedAppBar_frameLayout, _encryptedBarFragment).Hide(_encryptedBarFragment)
                .Add(Resource.Id.bottomBar_frameLayout, _encryptedBarBottomFragment).Show(_encryptedBarBottomFragment)
                .Add(Resource.Id.filenameFragment_frameLayout, _filenameFragment).Show(_filenameFragment)
                .Add(Resource.Id.passwordFragment_frameLayout, _passwordFragment).Show(_passwordFragment)
                .Commit();

            FindViewById<FrameLayout>(Resource.Id.filenameFragment_frameLayout).Visibility = ViewStates.Gone;
            FindViewById<FrameLayout>(Resource.Id.passwordFragment_frameLayout).Visibility = ViewStates.Gone;



        }

        public void DisplayPasswordDialog(SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSetWhenDialogOpened, Action<bool> setResult)
        {
            _passwordFragment.InitViewModel(setPasswordDialogMode, setIsPasswordSet, isPasswordSetWhenDialogOpened, setResult);
            FindViewById<FrameLayout>(Resource.Id.passwordFragment_frameLayout).Visibility = ViewStates.Visible;
        }


        public void HidePasswordDialog()
        {
            FindViewById<FrameLayout>(Resource.Id.passwordFragment_frameLayout).Visibility = ViewStates.Gone;
        }
        public void DisplayFilenameDialog(FileDialogMode saveAs, string[] filenames, Action<Tuple<bool, string>> setResult)
        {
            _filenameFragment.InitViewModel(FileDialogMode.SaveAs, filenames, setResult);
            FindViewById<FrameLayout>(Resource.Id.filenameFragment_frameLayout).Visibility = ViewStates.Visible;
        }

        public void HideFilenameDialog()
        {
            FindViewById<FrameLayout>(Resource.Id.filenameFragment_frameLayout).Visibility = ViewStates.Gone;
        }







        protected override void OnResume()
        {
            Console.WriteLine("MainActivity.OnResume");
            base.OnResume();
            MainActivityInstance = this;
            _mainViewModel.StatusBarModel.PropertyChanged += StatusBarModel_PropertyChanged;
        }

        protected override void OnPause()
        {
            Console.WriteLine("MainActivity.OnPause");
            base.OnPause();
            _mainViewModel.StatusBarModel.PropertyChanged -= StatusBarModel_PropertyChanged;
        }

        protected override void OnStop()
        {
            Console.WriteLine("MainActivity.OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("MainActivity.OnDestroy");
            base.OnDestroy();
        }



        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Console.WriteLine("MainActivity.OnOptionsItemSelected");

            Task menuTask = null;

            switch (item.ItemId)
            {
                case Resource.Id.action_encrypt:
                    if (_mainViewModel.EncryptEditorContentsCommand.CanExecute())
                        menuTask = _mainViewModel.EncryptEditorContentsCommand.Execute();
                    break;
                case Resource.Id.action_decrpyt:
                    if (_mainViewModel.DecryptEditorContentsCommand.CanExecute())
                        menuTask = _mainViewModel.DecryptEditorContentsCommand.Execute();
                    break;
                case Resource.Id.action_save:
                    if (_mainViewModel.SaveCommand.CanExecute())
                        menuTask = _mainViewModel.SaveCommand.Execute();
                    break;
                default:
                    return false;

            }
            if (menuTask != null)
                menuTask.ConfigureAwait(false);
            return true;


        }


    }
}


