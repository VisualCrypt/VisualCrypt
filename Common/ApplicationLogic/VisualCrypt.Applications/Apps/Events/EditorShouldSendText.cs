using System;
using Prism.Events;

namespace VisualCrypt.Applications.Portable.Apps.Events
{
    public class EditorShouldSendText : PubSubEvent<Action<string>>
	{
	}
}