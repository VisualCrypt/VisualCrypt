using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Applications.Portable.Apps.Events
{
	public class EditorSendsStatusBarInfo : PubSubEvent<string>
	{
	}
}