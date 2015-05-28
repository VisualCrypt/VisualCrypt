using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Desktop.Shared.Events
{

    public class EditorShouldSendText : PubSubEvent<Action<string>> { }
}
