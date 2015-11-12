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
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using System.Text;
using VisualCrypt.Language.Strings;
using Windows.UI.ViewManagement;
using VisualCrypt.Windows.Styles;

namespace VisualCrypt.Windows.Pages
{
    public sealed partial class MainPagePhone
    {
        static MainPagePhone _pageReference;

        readonly PortableMainViewModel _viewModel;
        readonly SettingsManager _settingsManager;
        DataTransferManager _dataTransferManager;
        ShareOperation shareOperation;

        private string sharedDataTitle;
        private string sharedDataDescription;
        private string sharedDataPackageFamilyName;
        private Uri sharedDataContentSourceWebLink;
        private Uri sharedDataContentSourceApplicationLink;
        private Color sharedDataLogoBackgroundColor;
        private IRandomAccessStreamReference sharedDataSquare30x30Logo;
        private string shareQuickLinkId;
        private string sharedText;
        private Uri sharedWebLink;
        private Uri sharedApplicationLink;
        private IReadOnlyList<IStorageItem> sharedStorageItems;
        private string sharedCustomData;
        private string sharedHtmlFormat;
        private IReadOnlyDictionary<string, RandomAccessStreamReference> sharedResourceMap;
        private IRandomAccessStreamReference sharedBitmapStreamRef;
        private IRandomAccessStreamReference sharedThumbnailStreamRef;
        private const string dataFormatName = "http://schema.org/Book";


        public MainPagePhone()
        {
            InitializeComponent();
            _viewModel = Service.Get<PortableMainViewModel>();
            _settingsManager = (SettingsManager)Service.Get<AbstractSettingsManager>();
            _pageReference = this;
            var ResourceWrapper = Service.Get<ResourceWrapper>();
            var localized = ResourceWrapper.miHelp;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        Dictionary<AppBarButton, double> buttonDictionary;
        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if(buttonDictionary == null)
            {
                buttonDictionary = new Dictionary<AppBarButton, double>();
                foreach (AppBarButton button in TopAppBar.PrimaryCommands)
                {
                    buttonDictionary.Add(button, button.Width);

                }
            }

            if (App.IsLandscape(e))
            {
                foreach (AppBarButton button in TopAppBar.PrimaryCommands)
                {
                    button.Width = 68;

                }
            }
            else
            {
                foreach (var entry in buttonDictionary)
                {
                    entry.Key.Width = entry.Value;
                }
            }
        }

        void DisplayDialogCommon()
        {
            TopAppBar.Visibility = Visibility.Collapsed;
            ExtendedAppBar.Visibility = Visibility.Collapsed;

            EditorUserControl.TextBox1.IsEnabled = false;
            EditorUserControl.TextBox1.Opacity = 0.8;

            BottomBar.Opacity = 0.8;
            BackgroundGrid.Background = MoreColors.BackgroundGridDisabledColorBrush;
        }

        void HideDialogCommon()
        {
            TopAppBar.Visibility = Visibility.Visible;
            ExtendedAppBar.Visibility = Visibility.Visible;
            EditorUserControl.TextBox1.IsEnabled = true;
            EditorUserControl.TextBox1.Opacity = 1;
            EditorUserControl.TextBox1.Focus(FocusState.Programmatic);
            BottomBar.Opacity = 1;
            BackgroundGrid.Background = MoreColors.WhiteBrush;
        }

        internal void DisplayPasswordDialog()
        {
            DisplayDialogCommon();
            PasswordUserControl.Visibility = Visibility.Visible;
        }

        internal void HidePasswordDialog()
        {
            HideDialogCommon();
            PasswordUserControl.Visibility = Visibility.Collapsed;
        }

        internal void DisplayFilenameDialog()
        {
            DisplayDialogCommon();
            FilenameUserControl.Visibility = Visibility.Visible;
        }

        internal void HideFilenameDialog()
        {
            HideDialogCommon();
            FilenameUserControl.Visibility = Visibility.Collapsed;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ShareOperation)
            {
                shareOperation = (ShareOperation)e.Parameter;
                await Task.Factory.StartNew(async () =>
                {
                    // Retrieve the data package properties.
                    this.sharedDataTitle = this.shareOperation.Data.Properties.Title;
                    this.sharedDataDescription = this.shareOperation.Data.Properties.Description;
                    this.sharedDataPackageFamilyName = this.shareOperation.Data.Properties.PackageFamilyName;
                    this.sharedDataContentSourceWebLink = this.shareOperation.Data.Properties.ContentSourceWebLink;
                    this.sharedDataContentSourceApplicationLink = this.shareOperation.Data.Properties.ContentSourceApplicationLink;
                    this.sharedDataLogoBackgroundColor = this.shareOperation.Data.Properties.LogoBackgroundColor;
                    this.sharedDataSquare30x30Logo = this.shareOperation.Data.Properties.Square30x30Logo;
                    this.sharedThumbnailStreamRef = this.shareOperation.Data.Properties.Thumbnail;
                    this.shareQuickLinkId = this.shareOperation.QuickLinkId;

                    // Retrieve the data package content.
                    // The GetWebLinkAsync(), GetTextAsync(), GetStorageItemsAsync(), etc. APIs will throw if there was an error retrieving the data from the source app.
                    // In this sample, we just display the error. It is recommended that a share target app handles these in a way appropriate for that particular app.

                    if (this.shareOperation.Data.Contains(StandardDataFormats.Text))
                    {
                        try
                        {
                            this.sharedText = await this.shareOperation.Data.GetTextAsync();
                        }
                        catch (Exception ex)
                        {
                            NotifyUserBackgroundThread("Failed GetTextAsync - " + ex.Message);
                        }
                    }


                    // In this sample, we just display the shared data content.

                    // Get back to the UI thread using the dispatcher.
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                        {
                                            EditorUserControl.TextBox1.Text = sharedText;

                                        });
                });
            }
            base.OnNavigatedTo(e);
            RoutedEventHandler handler = null;
            handler = async (sender, args) =>
                        {
                            Loaded -= handler;
                            await _viewModel.OnNavigatedToCompletedAndLoaded((FilesPageCommandArgs)e.Parameter);
                        };
            Loaded += handler;
            // Register the current page as a share source.
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(OnDataRequested);
        }

        async private void NotifyUserBackgroundThread(string message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NotifyUser(message);
            });
        }

        private void NotifyUser(string strMessage)
        {
            // do it
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(OnDataRequested);
        }

        /// <summary>
        /// Used to retrieve the Canvas for printing (x:Name="PrintCanvas").
        /// </summary>
        /// <returns></returns>
        public static MainPagePhone GetPageReference()
        {
            return _pageReference;
        }

        // When share is invoked (by the user or programatically) the event handler we registered will be called to populate the datapackage with the
        // data to be shared.
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            if (GetShareContent(e.Request))
            {

            }
        }

        bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            string dataPackageText = EditorUserControl.TextBox1.Text;
            if (!String.IsNullOrEmpty(dataPackageText))
            {
                DataPackage requestData = request.Data;
                requestData.Properties.Title = "VisualCrypt";
                //requestData.Properties.Description = DescriptionInputBox.Text; // The description is optional.
                requestData.Properties.ContentSourceApplicationLink = ApplicationLink;
                requestData.SetText(dataPackageText);
                succeeded = true;
            }
            else
            {
                request.FailWithDisplayText("Enter the text you would like to share and try again.");
            }
            return succeeded;
        }

        Uri ApplicationLink
        {
            get
            {
                return new Uri("ms-sdk-sharesourcecs:navigate?page=" + GetType().Name);
            }
        }

        private void abbShare_Click(object sender, RoutedEventArgs e)
        {
            // If the user clicks the share button, invoke the share flow programatically.
            DataTransferManager.ShowShareUI();
        }

        private void AppBarButton_Print_Click(object sender, RoutedEventArgs e)
        {
            if (EditorUserControl._viewModel.CanExecutePrint())
                EditorUserControl._viewModel.ExecutePrint();
        }
    }
}
