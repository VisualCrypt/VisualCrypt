using System;
using Windows.Graphics.Printing;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Printing;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.UWP.Pages;

namespace VisualCrypt.UWP.Services.Printing
{
    class Printer : IPrinter
    {
        readonly IMessageBoxService _messageBoxService;
        PrintHelper _printHelper;
        Page _callerPageReference;
        public Printer()
        {
            _messageBoxService = Service.Get<IMessageBoxService>();
        }

        public async void PrintEditorText(string editorText)
        {
            try
            {
                _callerPageReference = MainPagePhone.PageReference;
                _printHelper = new PrintHelper(MainPagePhone.PageReference, _messageBoxService, OnPrintTaskCompleted);

                var pageToPrint = new PageToPrint();
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run { Text = editorText });
                pageToPrint.TextContentBlock.Blocks.Add(paragraph);

                _printHelper.PreparePrintContent(pageToPrint);

                // does not block, throws if printing is not supported on the device.
                await PrintManager.ShowPrintUIAsync();

            }
            catch (Exception e)
            {
                await _messageBoxService.ShowError(e);
                try
                {
                    _printHelper.UnregisterForPrinting();
                }
                catch (Exception) { }
            }

        }

        async void OnPrintTaskCompleted(PrintTask s, PrintTaskCompletedEventArgs args)
        {

            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                await _callerPageReference.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await _messageBoxService.ShowError("Failed to print."); });
            }

            _printHelper.UnregisterForPrinting();
        }
    }


}
