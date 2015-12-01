using Android.OS;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using Fragment = Android.Support.V4.App.Fragment;

namespace VisualCrypt.Views
{
    class PlaintextBarBottomFragment : Fragment
    {
        readonly PortableMainViewModel _mainViewModel;
        public PlaintextBarBottomFragment()
        {
            _mainViewModel = Service.Get<PortableMainViewModel>();
           
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

        private void StatusBarModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_mainViewModel.StatusBarModel.StatusBarText))
            {
                View.FindViewById<TextView>(Resource.Id.plainTextBarText).Text =
                    _mainViewModel.StatusBarModel.StatusBarText;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.PlaintextBarBottomFragment, container, false);
        }
    }
}