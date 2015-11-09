using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using VisualCrypt.Applications;
using VisualCrypt.Applications.Models;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.Windows.Services;
using Windows.UI.Xaml.Controls;
using VisualCrypt.Applications.Services.PortableImplementations;
using System;
using VisualCrypt.Applications.Constants;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPagePhone
    {
        readonly PortableMainViewModel _viewModel;
        readonly SettingsManager _settingsManager;
        static MainPagePhone _pageReference;

        public MainPagePhone()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _pageReference = this;
        }

        internal void DisplayPasswordDialog()
        {
            TopAppBar.Visibility = Visibility.Collapsed;
            ExtendedAppBar.Visibility = Visibility.Collapsed;
            PasswordUserControl.Visibility = Visibility.Visible;
         
            EditorUserControl.TextBox1.IsEnabled = false;
            EditorUserControl.TextBox1.Opacity = 0.8;

            BottomBar.Opacity = 0.5;
        }

        internal void HidePasswordDialog()
        {
            TopAppBar.Visibility = Visibility.Visible;
            ExtendedAppBar.Visibility = Visibility.Visible;
            PasswordUserControl.Visibility = Visibility.Collapsed;

            EditorUserControl.TextBox1.IsEnabled = true;
            EditorUserControl.TextBox1.Opacity = 1;
            EditorUserControl.TextBox1.Focus(FocusState.Programmatic);
            BottomBar.Opacity = 1;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RoutedEventHandler handler = null;
            handler = async (sender, args) =>
            {
                Loaded -= handler;
                await _viewModel.OnNavigatedToCompletedAndLoaded((FilesPageCommandArgs)e.Parameter);
            };
            Loaded += handler;
        }

        /// <summary>
        /// Used to retrieve the Canvas for printing (x:Name="PrintCanvas").
        /// </summary>
        /// <returns></returns>
        public static MainPagePhone GetMainPageReference()
        {
            return _pageReference;
        }


    }
}
