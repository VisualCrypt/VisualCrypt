using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Windows.Events
{
	public class EditorShouldCleanup: PubSubEvent<EditorShouldCleanup>
	{
	}
}