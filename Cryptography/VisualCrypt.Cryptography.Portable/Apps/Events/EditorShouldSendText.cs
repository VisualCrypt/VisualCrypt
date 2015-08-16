using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Cryptography.Portable.Apps.Events
{
	public class EditorShouldSendText : PubSubEvent<Action<string>>
	{
	}
}