using System;
using System.Threading;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public class LongRunningOperationContext : IDisposable
	{
		readonly CancellationTokenSource CancellationTokenSource;

		public LongRunningOperationContext()
		{
			CancellationTokenSource = new CancellationTokenSource();
			CancellationToken = CancellationTokenSource.Token;
		}
		

		public string Description { get; set; }

		public IProgress<int> Progress { get; set; }

		public CancellationToken CancellationToken { get; set; }

		public void Cancel()
		{
			CancellationTokenSource.Cancel();
		}

		public void Dispose()
		{
			CancellationTokenSource.Dispose();
		}
	}
}
