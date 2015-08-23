using System;
using Prism.Events;

namespace VisualCrypt.Applications.Portable.Apps.Events
{
    public class EditorSendsText : PubSubEvent<EditorSendsText>
	{
		public string Text { get; set; }
		public Action<string> Callback { get; set; }
	}
}