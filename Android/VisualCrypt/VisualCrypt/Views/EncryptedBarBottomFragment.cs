using System.ComponentModel;
using Android.OS;
using Android.Views;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Widget;

namespace VisualCrypt.Views
{
    class EncryptedBarBottomFragment : Fragment
    {
        readonly PortableMainViewModel _mainViewModel;
        public EncryptedBarBottomFragment()
        {
            _mainViewModel = Service.Get<PortableMainViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.EncryptedBarBottomFragment, container, false);
        }

        public override void OnResume()
        {
            base.OnResume();
            _mainViewModel.StatusBarModel.PropertyChanged += StatusBarModel_PropertyChanged;
        }

        public override void OnPause()
        {
            base.OnPause();
            _mainViewModel.StatusBarModel.PropertyChanged -= StatusBarModel_PropertyChanged;
        }

        private void StatusBarModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(_mainViewModel.StatusBarModel.EncrytedBarText))
                View.FindViewById<TextView>(Resource.Id.encryptedBarBottomText).Text =
                    _mainViewModel.StatusBarModel.EncrytedBarText;
        }

      
    }
}