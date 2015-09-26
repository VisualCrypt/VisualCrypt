using System;
using System.Globalization;
using System.Windows;
using Prism.Logging;
using VisualCrypt.Applications.Services.PortableImplementations;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;

namespace VisualCrypt.Desktop.Views
{

    public partial class LogWindow
    {
        ResourceWrapper ResourceWrapper;
        ReplayLogger _logger;

        public LogWindow()
        {
            ResourceWrapper = Service.Get<ResourceWrapper>();
            // DataBinding would work perfectly, but the NoDots() extension requires working with a method.
            ResourceWrapper.Info.CultureChanged += (s, e) => { SetTexts(); }; 
            _logger = Service.Get<ILog>() as ReplayLogger;
            InitializeComponent();
            Loaded += LogWindow_Loaded;
            Closed += LogWindow_Closed;
        }

        void LogWindow_Closed(object sender, EventArgs e)
        {
            _logger.Callback = null;
        }

        void LogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _logger.Callback = Log;
            _logger.ReplaySavedLogs(Log);
            SetTexts();
        }

        void Log(string message, Category category, Priority priority)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                TraceTextBox.AppendText(
                       string.Format(
                           CultureInfo.CurrentUICulture,
                           "[{0}][{1}] {2}\r\n",
                           category,
                           priority,
                           message));

                TraceTextBox.ScrollToEnd();
            }));

        }


        void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void SetTexts()
        {
            ButtonClose.Content = ResourceWrapper.termClose;
            TextBlockInfo.Text = ResourceWrapper.logWindowInfoText;
            H1.Text = ResourceWrapper.miHelpLog.NoDots();
        }
    }
}