using System;
using System.Threading;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public class LongRunningOperationContext
	{
		public IProgress<int> Progress { get; set; }

		public CancellationToken CancellationToken { get; set; }
		
	}
}
