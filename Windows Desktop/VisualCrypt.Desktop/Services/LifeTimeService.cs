using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Desktop.Services
{
	public class LifeTimeService :ILifeTimeService
	{
		bool _isExitConfirmed;

        public void HandleExitRequested(CancelEventArgs e, Func<Task<bool>> confirmDiscard)
        {
            throw new NotImplementedException();
        }

        public void HandleExitRequested(CancelEventArgs e, Func<bool> confirmDiscard)
		{
			bool isInvokedFromWindowCloseEvent = e != null;

			if (isInvokedFromWindowCloseEvent)
			{
				if (_isExitConfirmed)
					return;
				if (confirmDiscard())
					return;
				e.Cancel = true;
			}
			else
			{
				if (confirmDiscard())
				{
					_isExitConfirmed = true;
					Application.Current.Shutdown();
				}
			}
		}
	}
}
