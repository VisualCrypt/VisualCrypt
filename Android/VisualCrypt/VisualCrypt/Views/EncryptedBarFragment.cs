using Android.OS;
using Android.Views;
using Android.Widget;
using System.ComponentModel;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Language.Strings;
using Fragment = Android.Support.V4.App.Fragment;

namespace VisualCrypt.Views
{
    class EncryptedBarFragment : Fragment
    {
        readonly ResourceWrapper _resourceWrapper;
        readonly PortableMainViewModel _mainViewModel;

        TextView _textViewPasswordStatus;
        TextView _textViewClearPassword;
        TextView _textViewCopyAll;

        public EncryptedBarFragment()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _mainViewModel = Service.Get<PortableMainViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view =  inflater.Inflate(Resource.Layout.EncryptedBarFragment, container, false);
            _textViewPasswordStatus = view.FindViewById<TextView>(Resource.Id.encryptedBarFragment_textViewPasswordStatus);
            _textViewPasswordStatus.Click += async (s, e) =>
            {
                await _mainViewModel.ShowSetPasswordDialogCommand.Execute();
            };
            _textViewClearPassword = view.FindViewById<TextView>(Resource.Id.encryptedBarFragment_textViewClearPassword);
            _textViewClearPassword.Click += async (s, e) =>
            {
                await _mainViewModel.ClearPasswordCommand.Execute();
            };
            _textViewCopyAll = view.FindViewById<TextView>(Resource.Id.encryptedBarFragment_textViewCopyAll);
            _textViewCopyAll.Click += async (s, e) =>
            {
                await _mainViewModel.CopyAllCommand.Execute();
            };
            return view;
        }
        public override void OnResume()
        {
            base.OnResume();
            _mainViewModel.PasswordInfo.PropertyChanged -= OnPasswordInfoPropertyChanged;
            _mainViewModel.PasswordInfo.PropertyChanged += OnPasswordInfoPropertyChanged;
            OnPasswordInfoPropertyChanged(null, null);
        }



        public override void OnPause()
        {
            base.OnPause();
            _mainViewModel.PasswordInfo.PropertyChanged -= OnPasswordInfoPropertyChanged;
        }

        void OnPasswordInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _textViewPasswordStatus.Text = _mainViewModel.PasswordInfo.PasswordStatus;
            _textViewCopyAll.Text = _resourceWrapper.termCopyToClipboard;
            if (_mainViewModel.PasswordInfo.IsPasswordSet)
                _textViewClearPassword.Visibility = ViewStates.Visible;
            else
                _textViewClearPassword.Visibility = ViewStates.Invisible;
        }
    }
}