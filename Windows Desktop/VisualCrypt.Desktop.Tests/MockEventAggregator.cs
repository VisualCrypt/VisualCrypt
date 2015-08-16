using Microsoft.Practices.Prism.PubSubEvents;

namespace VisualCrypt.Desktop.Tests
{
    public class MockEventAggregator : IEventAggregator
	{
		public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
		{
			return default(TEventType);
		}
	}
}
