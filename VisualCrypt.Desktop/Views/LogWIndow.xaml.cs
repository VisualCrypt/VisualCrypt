// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.Practices.Prism.Logging;

namespace VisualCrypt.Desktop.Views
{
    [Export]
    public partial class LogWindow 
    {
        [Import(AllowRecomposition = false)]
        CallbackLogger _logger;

        public LogWindow()
        {
            InitializeComponent();
        }

        public void Log(string message, Category category, Priority priority)
        {
            TraceTextBox.AppendText(
                string.Format(
                    CultureInfo.CurrentUICulture,
                    "[{0}][{1}] {2}\r\n",
                    category,
                    priority,
                    message));
        }

     
        public void OnImportsSatisfied()
        {
            _logger.ReplaySavedLogs(Log);
        }
    }
}