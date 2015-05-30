using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Desktop.Shared.Events
{
	public class EditorReceivesText : PubSubEvent<string>
	{
	}
}