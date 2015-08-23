using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using VisualCrypt.Applications.Portable.Apps.Services;

namespace VisualCrypt.Desktop.Shared.Services
{
	[Export(typeof(ILifeTimeService))]
	public class LifeTimeService :ILifeTimeService
	{
		bool _isExitConfirmed;

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
