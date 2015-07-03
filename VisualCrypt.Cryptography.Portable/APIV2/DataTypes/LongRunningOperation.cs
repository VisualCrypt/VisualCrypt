﻿using System;
using System.Threading;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public class LongRunningOperation : IDisposable
	{
		readonly CancellationTokenSource _cancellationTokenSource;
		readonly LongRunningOperationContext _context;
		readonly Action _switchBackToPreviousBar;

		public LongRunningOperation(Action<int> reportAction, Action switchBackToPreviousBar)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_switchBackToPreviousBar = switchBackToPreviousBar;
			_context = new LongRunningOperationContext(_cancellationTokenSource.Token, new Progress<int>(reportAction));
		}
		public LongRunningOperationContext Context
		{
			get { return _context; }
		}


		public void Cancel()
		{
			_cancellationTokenSource.Cancel();
			_switchBackToPreviousBar();
		}

		public void Dispose()
		{
			_cancellationTokenSource.Dispose();
			
		}
	}
}
