using Microsoft.Practices.Prism.PubSubEvents;
using VisualCrypt.Cryptography.Portable;
using VisualCrypt.Cryptography.Portable.Settings;
using VisualCrypt.Windows.Services;

namespace VisualCrypt.Windows.Static
{
    static class SharedInstances
    {
        public static readonly IEventAggregator EventAggregator = new EventAggregator();

        public static readonly IMessageBoxService MessageBoxService = new MessageBoxService();

        public static readonly ISettingsManager SettingsManager = new SettingsManager();
    }
}
