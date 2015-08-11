using System;
using System.ComponentModel;

namespace VisualCrypt.Cryptography.Portable
{
	public interface ILifeTimeService
	{
		void HandleExitRequested(CancelEventArgs e, Func<bool> confirmDiscard);
	}
}
