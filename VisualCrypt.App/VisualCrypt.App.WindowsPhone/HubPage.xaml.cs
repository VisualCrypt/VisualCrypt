using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using VisualCrypt.App.API.V2;
using VisualCrypt.App.Common;
using VisualCrypt.App.Data;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VisualCrypt.App
{
    public sealed partial class HubPage : IFileOpenPickerContinuable
    {
        
        readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        readonly ResourceLoader _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public HubPage()
        {
            InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

      

       

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Details about the click event.</param>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(SectionPage), groupId))
            {
                throw new Exception(this._resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
        
        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">Defaults about the click event.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this._resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

     
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
           
        }

       
        private void ButtonNew_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(EditorPage), null))
            {
                throw new Exception(this._resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        async void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                //var dtm = DataTransferManager.GetForCurrentView();
                //dtm.TargetApplicationChosen += dtm_TargetApplicationChosen;
                //dtm.DataRequested += dtm_DataRequested;
                //Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
                //await Windows.System.Launcher.LaunchUriAsync(new Uri("whatsapp:send?text=Hi"));

                var openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".visualcrypt");
                openPicker.FileTypeFilter.Add(".txt");

                openPicker.PickSingleFileAndContinue();
                
            }

        }

        void dtm_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            var deferral = request.GetDeferral();
            request.Data.Properties.Title = "Exmaple Title";
            request.Data.Properties.Description = "Ex Description";
            request.Data.SetText("Hello World Message");
           deferral.Complete();
        }

        void dtm_TargetApplicationChosen(DataTransferManager sender, TargetApplicationChosenEventArgs args)
        {
            var chosen = args.ApplicationName;
        }

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                StorageFile file = args.Files[0];

	            var svc = new EncryptionService();
	            var response = svc.OpenFile(file.Path +file.Name);


	            ModelState.TextBuffer = response.Result.VisualCryptText;

                if (!Frame.Navigate(typeof(EditorPage), null))
                {
                    throw new Exception(this._resourceLoader.GetString("NavigationFailedExceptionMessage"));
                }


            }
            else
            {
                TextBlockOutput.Text = "Operation cancelled.";
            }
        }


    }
}