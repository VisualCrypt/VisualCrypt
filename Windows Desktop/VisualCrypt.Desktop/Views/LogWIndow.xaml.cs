using System;
using System.Globalization;
using System.Windows;
using Prism.Logging;
using VisualCrypt.Applications.Services.PortableImplementations;

namespace VisualCrypt.Desktop.Views
{
    
    public partial class LogWindow
    {
       ReplayLogger _logger;

        public LogWindow()
        {
            _logger = new ReplayLogger();
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
        }

        void Log(string message, Category category, Priority priority)
        {
            TraceTextBox.AppendText(
                string.Format(
                    CultureInfo.CurrentUICulture,
                    "[{0}][{1}] {2}\r\n",
                    category,
                    priority,
                    message));

            TraceTextBox.ScrollToEnd();
        }


        void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}