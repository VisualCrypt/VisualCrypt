using System;
using Prism.Events;

namespace VisualCrypt.Applications.Apps.Events
{
    public class EditorShouldSendText : PubSubEvent<Action<string>>
	{
	}
}