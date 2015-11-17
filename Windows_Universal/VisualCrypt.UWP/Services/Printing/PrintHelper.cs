using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Applications.ViewModels;
using VisualCrypt.UWP.Pages;

namespace VisualCrypt.UWP.Services.Printing
{
    sealed class PrintHelper
    {
        const double ApplicationContentMarginLeft = 0.075;  // %
        const double ApplicationContentMarginTop = 0.03; // %

        readonly List<UIElement> _printPreviewPages;
        readonly Page _callerPageReference;  // has a hidden Canvas named '_printCanvas', provides the Dispatcher
        readonly Canvas _printCanvas;
        readonly IMessageBoxService _messageBoxService;
        readonly TypedEventHandler<PrintTask, PrintTaskCompletedEventArgs> _onPrintTaskCompleted;

        readonly PrintDocument _printDocument;  // Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        IPrintDocumentSource _printDocumentSource;  // Marker interface for document source
        FrameworkElement _firstPage; // From this "virtual sized" paged content is split(text is flowing) to "printing pages"
       
        public event EventHandler PreviewPagesCreated; // Event callback which is called after print preview pages are generated.  Photos scenario uses this to do filtering of preview pages

        public PrintHelper(Page callerPageReference, IMessageBoxService messageBoxService, TypedEventHandler<PrintTask, PrintTaskCompletedEventArgs> onPrintTaskCompleted)
        {
            _printCanvas = callerPageReference.FindName("PrintCanvas") as Canvas;
            if(_printCanvas == null)
                throw  new Exception("The element 'PrintCanvas' is missing on the page that initiated printing.");

            _messageBoxService = messageBoxService;
            _onPrintTaskCompleted = onPrintTaskCompleted;
            _callerPageReference = callerPageReference;
            _printPreviewPages = new List<UIElement>();
            
            // Register for Printing
            _printDocument = new PrintDocument();
            _printDocumentSource = _printDocument.DocumentSource;
            _printDocument.Paginate += CreatePrintPreviewPages;
            _printDocument.GetPreviewPage += GetPrintPreviewPage;
            _printDocument.AddPages += AddPrintPages;

            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

      

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        public async void UnregisterForPrinting()
        {
            if (_printDocument == null)
            {
                return;
            }
            await _callerPageReference.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _printDocument.Paginate -= CreatePrintPreviewPages;
                _printDocument.GetPreviewPage -= GetPrintPreviewPage;
                _printDocument.AddPages -= AddPrintPages;
                // Remove the handler for printing initialization.
                PrintManager printMan = PrintManager.GetForCurrentView();
                printMan.PrintTaskRequested -= PrintTaskRequested;

                _printCanvas.Children.Clear();
            });
           

           
        }

        public async Task ShowPrintUIAsync()
        {
            // Catch and print out any errors reported
            try
            {
                await PrintManager.ShowPrintUIAsync();
            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError("Error printing: " + e.Message + ", hr=" + e.HResult);
            }
        }

        /// <summary>
        /// Method that will generate print content for the scenario
        /// For scenarios 1-4: it will create the first page from which content will flow
        /// Scenario 5 uses a different approach
        /// </summary>
        /// <param name="page">The page to print</param>
        public void PreparePrintContent(Page page)
        {
            if (_firstPage == null)
            {
                _firstPage = page;
                StackPanel header = (StackPanel)_firstPage.FindName("Header");
                if (header == null)
                    throw new Exception("Can't find element 'Header'");
                header.Visibility = Visibility.Visible;
            }

            // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            _printCanvas.Children.Add(_firstPage);
            _printCanvas.InvalidateMeasure();
            _printCanvas.UpdateLayout();
        }

       
         void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
         {
             var title = Service.Get<PortableMainViewModel>().FileModel.ShortFilename;
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask(title, sourceRequested =>
            {
                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += _onPrintTaskCompleted;

                sourceRequested.SetSource(_printDocumentSource);
            });
        }

       



        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            _printPreviewPages.Clear();
            _printCanvas.Children.Clear();
            var printingOptions = e.PrintTaskOptions;
            var pageDescription = printingOptions.GetPageDescription(0);

            // We know there is at least one page to be printed. passing null as the first parameter to
            // AddOnePrintPreviewPage tells the function to add the first page.
            var lastRtboOnPage = AddOnePrintPreviewPage(null, pageDescription);

            // We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
            // page has extra content
            while (lastRtboOnPage.HasOverflowContent && lastRtboOnPage.Visibility == Visibility.Visible)
            {
                lastRtboOnPage = AddOnePrintPreviewPage(lastRtboOnPage, pageDescription);
            }

            // Event callback which is called after print preview pages are generated.  
            // SDK Photos scenario uses this to do filtering of preview pages.
            if (PreviewPagesCreated != null)
                PreviewPagesCreated.Invoke(_printPreviewPages, null);

            var printDoc = (PrintDocument)sender;

            // Report the number of preview pages created
            printDoc.SetPreviewPageCount(_printPreviewPages.Count, PreviewPageCountType.Intermediate);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            var printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPage(e.PageNumber, _printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            foreach (var  page in _printPreviewPages)
                _printDocument.AddPage(page);

           // TODO: Check if this is bullshit
            var printDoc = (PrintDocument)sender;

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in _printPreviewPages.
        /// </summary>
        /// <param name="lastRtboAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRtboAdded, PrintPageDescription printPageDescription)
        {
            // XAML element that is used to represent to "printing page"
            FrameworkElement page;

            // The link container for text overflowing in this page

            // Check if this is the first page ( no previous RichTextBlockOverflow)
            if (lastRtboAdded == null)
            {
                // If this is the first page add the specific scenario content
                page = _firstPage;
                //Hide footer since we don't know yet if it will be displayed (this might not be the last page) - wait for layout
                var footer = (StackPanel)page.FindName("Footer");
                if (footer == null)
                    throw new Exception("Can't find element 'Footer'");
                footer.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Flow content (text) from previous pages
                page = new ContinuationPage(lastRtboAdded);
            }

            // Set "paper" width
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            var printableArea = (Grid)page.FindName("PrintableArea");
            if(printableArea == null)
                throw new Exception("Can't find Element 'PrintableArea'");

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            var marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
            var marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            // Set-up "printable area" on the "paper"
            printableArea.Width = _firstPage.Width - marginWidth;
            printableArea.Height = _firstPage.Height - marginHeight;

            // Add the (newley created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            _printCanvas.Children.Add(page);
            _printCanvas.InvalidateMeasure();
            _printCanvas.UpdateLayout();

            // Find the last text container and see if the content is overflowing
            var textLink = (RichTextBlockOverflow)page.FindName("ContinuationPageLinkedContainer");
            if(textLink == null)
                throw new Exception("Cant fint element 'ContinuationPageLinkedContainer'");
            // Check if this is the last page
            if (!textLink.HasOverflowContent && textLink.Visibility == Visibility.Visible)
            {
                StackPanel footer = (StackPanel)page.FindName("Footer");
                if (footer == null)
                    throw new Exception("Can't find element 'Footer'");
                footer.Visibility = Visibility.Visible;
            }

            // Add the page to the page preview collection
            _printPreviewPages.Add(page);

            return textLink;
        }
    }
}
