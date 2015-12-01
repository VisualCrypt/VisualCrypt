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
    class PasswordFragment : Fragment
    {
        readonly PortablePasswordDialogViewModel _passwordDialogViewModel;
        readonly ResourceWrapper _resourceWrapper;
        Button _okButton;
        public PasswordFragment()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
            _passwordDialogViewModel = Service.Get<PortablePasswordDialogViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.PasswordFragment, container, false);
            view.FindViewById<TextView>(Resource.Id.passwordFragment_textViewPasswordPhrase).Text = _resourceWrapper.spd_lbl_PasswordOrPhrase;
            view.FindViewById<Button>(Resource.Id.passwordFragment_buttonSuggestPW).Text = _resourceWrapper.spd_linktext_GeneratePassword;
            view.FindViewById<Button>(Resource.Id.passwordFragment_buttonPrintPW).Text = _resourceWrapper.spd_linktext_PrintPassword;
            view.FindViewById<TextView>(Resource.Id.passwordFragment_readMoreAboutPWs).Text = _resourceWrapper.spd_text_ReadMoreAbout;
            // To Do: configuere Link
            view.FindViewById<TextView>(Resource.Id.passwordFragment_buttonCancel).Text = _resourceWrapper.termCancel;
            view.FindViewById<Button>(Resource.Id.passwordFragment_buttonCancel).Click += (s, e) => {
                _passwordDialogViewModel.CancelCommand.Execute();
            };

           _okButton = view.FindViewById<Button>(Resource.Id.passwordFragment_buttonOK);
            _okButton.Click += (s, e) => {
                _passwordDialogViewModel.SetPasswordCommand.Execute();
            };
            _passwordDialogViewModel.SetPasswordCommand.CanExecuteChanged += (s, e) => {
                _okButton.Enabled = _passwordDialogViewModel.SetPasswordCommand.CanExecute();
            };
            view.FindViewById<EditText>(Resource.Id.passwordFragment_editTextPassword).TextChanged += (s, e) => {
                _passwordDialogViewModel.PasswordBoxText = ((EditText)s).Text;
            };
           
            return view;
        }

       

        public override void OnResume()
        {
            base.OnResume();
            _passwordDialogViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override void OnPause()
        {
            base.OnPause();
            _passwordDialogViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            View.FindViewById<TextView>(Resource.Id.passwordFragment_textViewTitle).Text = _passwordDialogViewModel.Title;
            _okButton.Text = _passwordDialogViewModel.OKButtonContent;
            _okButton.Enabled = _passwordDialogViewModel.SetPasswordCommand.CanExecute();
            View.FindViewById<TextView>(Resource.Id.passwordFragment_textViewSigCharCout).Text = _passwordDialogViewModel.SignificantCharCountText;
        }

        internal void InitViewModel(SetPasswordDialogMode setPasswordDialogMode, Action<bool> setIsPasswordSet, bool isPasswordSetWhenDialogOpened, Action<bool> setResult)
        {
            _passwordDialogViewModel.Init(setPasswordDialogMode, setIsPasswordSet, setResult, isPasswordSetWhenDialogOpened);
          
        }
    }
}