using System;
using System.ComponentModel;

namespace VisualCrypt.Applications.Apps.Services
{
	public interface ILifeTimeService
	{
		void HandleExitRequested(CancelEventArgs e, Func<bool> confirmDiscard);
	}
}
