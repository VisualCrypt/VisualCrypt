using System;
using VisualCrypt.Applications.Apps.Services;

namespace VisualCrypt.Windows.Services
{
    class Log : ILog
    {
        public void Debug(string info)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Debug: {0}", info));
        }

        public void Exception(Exception e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Exception: {0}", e.Message));
        }
    }
}
