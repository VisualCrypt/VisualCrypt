using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Cryptography.Portable.Events
{
	public class EditorSendsText : PubSubEvent<EditorSendsText>
	{
		public string Text { get; set; }
		public Action<string> Callback { get; set; }
	}
}