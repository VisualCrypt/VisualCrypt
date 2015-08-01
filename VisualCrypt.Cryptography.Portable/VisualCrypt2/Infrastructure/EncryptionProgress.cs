
using System;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public sealed class EncryptionProgress : Progress<EncryptionProgress>, IEncryptionProgress
	{
		public EncryptionProgress(Action<EncryptionProgress> reportAction) : base(reportAction)
		{
			if(reportAction == null)
				throw new ArgumentNullException("reportAction");
		}
		public int Percent { get; set; }

		public string Message { get; set; }


		public void Report(EncryptionProgress progress)
		{
			OnReport(progress);
		}
	}
}
