using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Logging;

namespace VisualCrypt.Desktop
{
    public class CallbackLogger : ILoggerFacade
    {
        readonly Queue<Tuple<string, Category, Priority>> _savedLogs =
            new Queue<Tuple<string, Category, Priority>>();


        Action<string, Category, Priority> _callback;


        public void Log(string message, Category category, Priority priority)
        {
            if (_callback != null)
            {
                _callback(message, category, priority);
            }
            else
            {
                _savedLogs.Enqueue(new Tuple<string, Category, Priority>(message, category, priority));
            }
        }

        public void ReplaySavedLogs(Action<string, Category, Priority> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            while (_savedLogs.Count > 0)
            {
                var log = _savedLogs.Dequeue();
                _callback(log.Item1, log.Item2, log.Item3);
            }
        }
    }
}