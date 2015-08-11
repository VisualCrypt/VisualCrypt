using System;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps;
using VisualCrypt.Desktop.Shared.App;
using VisualCrypt.Desktop.Shared.Settings;

namespace VisualCrypt.Desktop.ModuleEditor.FeatureSupport.Printing
{
	public static class Printer
	{
		public static FlowDocument ConvertToFlowDocument(string text)
		{
			var flowDocument = new FlowDocument {LineHeight = double.NaN};
			Map.Copy(SettingsManager.FontSettings, flowDocument);


			string[] lines;
			if (text.Contains("\n"))
				lines = text.Split('\n');
			else
			{
				lines = new string[1];
				lines[0] = text;
			}

			foreach (string line in lines)
			{
				var paragraphText = line.Replace("\r", "");
				var paragraph = new Paragraph
				{
					LineHeight = double.NaN,
					Margin = new Thickness(0),
					Padding = new Thickness(0)
				};

				paragraph.Inlines.Add(new Run(paragraphText));
				flowDocument.Blocks.Add(paragraph);
			}

			return flowDocument;
		}

		/// <summary>
		///     Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
		///     and allowing the user to select a printer.
		///     Gets information about the dimensions of the seleted printer + media.
		/// </summary>
		public static void PrintFlowDocument(FlowDocument flowDocument, int pagePadding)
		{
			PrintDocumentImageableArea printableArea = null;
			XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(ref printableArea);

			// now we have media information!

			if (xpsDocumentWriter != null && printableArea != null)
			{
				DocumentPaginator paginator = ((IDocumentPaginatorSource) flowDocument).DocumentPaginator;

				// Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
				paginator.PageSize = new Size(printableArea.MediaSizeWidth, printableArea.MediaSizeHeight);

				var t = new Thickness(72); // PagePadding;
				flowDocument.PagePadding = new Thickness(
					Math.Max(printableArea.OriginWidth, t.Left),
					Math.Max(printableArea.OriginHeight, t.Top),
					Math.Max(printableArea.MediaSizeWidth - (printableArea.OriginWidth + printableArea.ExtentWidth),
						t.Right),
					Math.Max(printableArea.MediaSizeHeight - (printableArea.OriginHeight + printableArea.ExtentHeight),
						t.Bottom));

				flowDocument.ColumnWidth = double.PositiveInfinity;

				// Send content to printer.
				xpsDocumentWriter.Write(paginator);
			}
		}
	}
}