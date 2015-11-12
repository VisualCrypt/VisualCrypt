using System;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml;

namespace VisualCrypt.Windows.Controls
{
    public sealed partial class FilenameUserControl : UserControl
    {
        PortableFilenameDialogViewModel _viewModel;
        public FilenameUserControl()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableFilenameDialogViewModel>();
            TextBoxFilename.TextChanged += TextBoxFilename_TextChanged;
            _viewModel.PropertyChanged += _viewModel_PropertyChanged;
        }

        private void _viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_viewModel.Filename))
                return;

            TextBoxFilename.Text = _viewModel.Filename;
            //TextBoxFilename.Select(TextBoxFilename.Text.Length,0);
        }

        private void TextBoxFilename_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Filename = TextBoxFilename.Text;
        }

        public void InitViewModel(FileDialogMode saveAs, string[] filenames, Action<Tuple<bool, string>> setResult)
        {
            _viewModel.Init(saveAs, filenames, setResult);
            TextBoxFilename.Focus(FocusState.Programmatic);
        }
    }
}
