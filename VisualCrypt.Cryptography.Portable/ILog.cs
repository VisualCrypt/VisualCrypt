using System;

namespace VisualCrypt.Cryptography.Portable
{
	public interface ILog
	{
		void Debug(string info);

		void Exception(Exception e);
	}
}
