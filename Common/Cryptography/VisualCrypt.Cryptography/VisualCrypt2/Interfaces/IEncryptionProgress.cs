using System;
using VisualCrypt.Cryptography.VisualCrypt2.Infrastructure;

namespace VisualCrypt.Cryptography.VisualCrypt2.Interfaces
{
	public interface IEncryptionProgress : IProgress<EncryptionProgress>
	{
		int Percent { get; set; }

		string Message { get; set; }
	}
}