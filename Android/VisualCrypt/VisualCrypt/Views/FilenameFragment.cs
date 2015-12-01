using System.ComponentModel;
using Android.OS;
using Android.Views;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Widget;
using VisualCrypt.Language.Strings;
using System;
using VisualCrypt.Applications.Models;

namespace VisualCrypt.Views
{
    class FilenameFragment : Fragment
    {
        readonly PortableFilenameDialogViewModel _filenameDialogViewModel;
        readonly ResourceWrapper _resourceWrapper;
        Button _okButton;
        public FilenameFragment()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _filenameDialogViewModel = Service.Get<PortableFilenameDialogViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FilenameFragment, container, false);
            view.FindViewById<TextView>(Resource.Id.filenameFragment_textViewFilename).Text = _resourceWrapper.termFilename;
            view.FindViewById<TextView>(Resource.Id.filenameFragment_buttonCancel).Text = _resourceWrapper.termCancel;
            view.FindViewById<Button>(Resource.Id.filenameFragment_buttonCancel).Click += (s, e) => {
                _filenameDialogViewModel.CancelCommand.Execute();
            };

           _okButton = view.FindViewById<Button>(Resource.Id.filenameFragment_buttonOK);
            _okButton.Click += (s, e) => {
                _filenameDialogViewModel.OKCommand.Execute();
            };
            _filenameDialogViewModel.OKCommand.CanExecuteChanged += (s, e) => {
                _okButton.Enabled = _filenameDialogViewModel.OKCommand.CanExecute();
            };
            view.FindViewById<EditText>(Resource.Id.filenameFragment_editTextFilename).TextChanged += (s, e) => {
                _filenameDialogViewModel.Filename = ((EditText)s).Text;
            };
           
            return view;
        }

       

        public override void OnResume()
        {
            base.OnResume();
            _filenameDialogViewModel.PropertyChanged += _filenameDialogViewModel_PropertyChanged;
        }

        public override void OnPause()
        {
            base.OnPause();
            _filenameDialogViewModel.PropertyChanged -= _filenameDialogViewModel_PropertyChanged;
        }

        private void _filenameDialogViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            View.FindViewById<TextView>(Resource.Id.filenameFragment_textViewTitle).Text = _filenameDialogViewModel.Title;
            _okButton.Text = _filenameDialogViewModel.OKButtonContent;
            _okButton.Enabled = _filenameDialogViewModel.OKCommand.CanExecute();
        }

        internal void InitViewModel(FileDialogMode saveAs, string[] filenames, Action<Tuple<bool, string>> setResult)
        {
            _filenameDialogViewModel.Init(saveAs, filenames, setResult);
          
        }
    }
}