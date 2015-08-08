using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Windows.Events
{
	public class EditorShouldSendText : PubSubEvent<Action<string>>
	{
	}
}