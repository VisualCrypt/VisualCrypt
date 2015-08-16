using System;
using System.Threading;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public class LongRunningOperationContext
	{
		readonly EncryptionProgress _encryptionProgress;
		readonly CancellationToken _cancellationToken;

		public LongRunningOperationContext(CancellationToken token, EncryptionProgress encryptionProgress)
		{
			if(encryptionProgress == null)
				throw new ArgumentNullException("encryptionProgress");
			_encryptionProgress = encryptionProgress;
			_cancellationToken = token;
		}

		public EncryptionProgress EncryptionProgress
		{
			get { return _encryptionProgress; }
		}

		public CancellationToken CancellationToken
		{
			get { return _cancellationToken; }
		}
	}
}