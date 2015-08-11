using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using Microsoft.Practices.Prism.Logging;

namespace VisualCrypt.Desktop.Views
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LogWindow
    {
        [Import(AllowRecomposition = false)] ReplayLogger _logger;

        public LogWindow()
        {
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