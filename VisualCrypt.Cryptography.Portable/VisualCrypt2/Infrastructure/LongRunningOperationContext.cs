using System;
using System.Threading;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public class LongRunningOperationContext
	{
		readonly IProgress<int> _progress;
		readonly CancellationToken _cancellationToken;

		public LongRunningOperationContext(CancellationToken token, IProgress<int> progress)
		{
			_progress = progress;
			_cancellationToken = token;
		}

		public IProgress<int> Progress
		{
			get { return _progress; }
		}

		public CancellationToken CancellationToken
		{
			get { return _cancellationToken; }
		}
	}
}