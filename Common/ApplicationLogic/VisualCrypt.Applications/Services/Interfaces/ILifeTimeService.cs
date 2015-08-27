using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace VisualCrypt.Applications.Services.Interfaces
{
	public interface ILifeTimeService
	{
		void HandleExitRequested(CancelEventArgs e, Func<Task<bool>> confirmDiscard);
	}
}
