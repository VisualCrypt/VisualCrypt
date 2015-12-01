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
    class WorkingBarFragment : Fragment
    {
        readonly ResourceWrapper _resourceWrapper;
        readonly PortableMainViewModel _mainViewModel;

        TextView _textViewCancel;
        TextView _textViewProgress1;
        TextView _textViewProgress2;
        ProgressBar _progressBar;

        public WorkingBarFragment()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _mainViewModel = Service.Get<PortableMainViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.WorkingBarFragment, container, false);
            _textViewCancel = view.FindViewById<TextView>(Resource.Id.workingBarFragment_textViewCancel);
            _textViewCancel.Click += (s, e) => { _mainViewModel.CancelLongRunningOperation(); }; 
            _textViewProgress1 = view.FindViewById<TextView>(Resource.Id.workingBarFragment_textViewProgress1);
            _textViewProgress2 = view.FindViewById<TextView>(Resource.Id.workingBarFragment_textViewProgress2);
            _progressBar = view.FindViewById<ProgressBar>(Resource.Id.workingBarFragment_progressBar);
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            _mainViewModel.StatusBarModel.PropertyChanged -= OnStatusBarModelPropertyChanged;
            _mainViewModel.StatusBarModel.PropertyChanged += OnStatusBarModelPropertyChanged;
            OnStatusBarModelPropertyChanged(null, null);
        }



        public override void OnPause()
        {
            base.OnPause();
            _mainViewModel.StatusBarModel.PropertyChanged -= OnStatusBarModelPropertyChanged;
        }

        void OnStatusBarModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _textViewCancel.Text = _resourceWrapper.termCancel;
            _textViewProgress1.Text = _mainViewModel.StatusBarModel.ProgressBarOpName;
            _textViewProgress2.Text = _mainViewModel.StatusBarModel.ProgressMessage;
            _progressBar.Progress = _mainViewModel.StatusBarModel.ProgressPercent;
            
        }
    }
}