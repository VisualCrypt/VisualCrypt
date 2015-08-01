using System;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public interface IEncryptionProgress : IProgress<EncryptionProgress>
	{
		int Percent { get; set; }

		string Message { get; set; }
	}
}