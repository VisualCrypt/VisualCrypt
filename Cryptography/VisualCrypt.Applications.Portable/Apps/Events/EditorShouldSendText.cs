using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Applications.Portable.Apps.Events
{
	public class EditorShouldSendText : PubSubEvent<Action<string>>
	{
	}
}