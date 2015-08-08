using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Windows.Services;

namespace VisualCrypt.Windows.Static
{
    static class SharedInstances
    {
        public static readonly IEventAggregator EventAggregator = new EventAggregator();

        public static readonly IMessageBoxService MessageBoxService = new MessageBoxService();
    }
}
