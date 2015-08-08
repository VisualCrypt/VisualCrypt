﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPage
    {
        readonly MainPageViewModel _viewModel = new MainPageViewModel();

        public MainPage()
        {
            InitializeComponent();
            Loaded += (s, e) => _viewModel.OnViewInitialized(Frame,_textBox1);
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.OnViewInitialized(Frame, _textBox1);
            base.OnNavigatedTo(e);
            _viewModel.OnNavigatedTo(e);
        }
    }
}
