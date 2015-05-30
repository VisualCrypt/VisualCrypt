using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Desktop.Shared.Events
{
	public class EditorSendsText : PubSubEvent<EditorSendsText>
	{
		public string Text { get; set; }
		public Action<string> Callback { get; set; }
	}
}