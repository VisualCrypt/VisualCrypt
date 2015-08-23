using System;
using System.ComponentModel;

namespace VisualCrypt.Applications.Portable.Apps.Services
{
	public interface ILifeTimeService
	{
		void HandleExitRequested(CancelEventArgs e, Func<bool> confirmDiscard);
	}
}
