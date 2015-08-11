using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Cryptography.Portable.Events
{
	public class EditorShouldSendText : PubSubEvent<Action<string>>
	{
	}
}