using System;
using Prism.Events;

namespace VisualCrypt.Applications.Events
{
    public class EditorShouldSendText : PubSubEvent<Action<string>>
	{
	}
}