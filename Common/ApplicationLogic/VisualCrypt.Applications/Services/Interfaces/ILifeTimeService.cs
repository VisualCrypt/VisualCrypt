using System;
using System.ComponentModel;

namespace VisualCrypt.Applications.Services.Interfaces
{
	public interface ILifeTimeService
	{
		void HandleExitRequested(CancelEventArgs e, Func<bool> confirmDiscard);
	}
}
