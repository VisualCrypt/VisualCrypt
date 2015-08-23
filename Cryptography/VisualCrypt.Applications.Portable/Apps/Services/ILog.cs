using System;

namespace VisualCrypt.Cryptography.Portable.Apps.Services
{
	public interface ILog
	{
		void Debug(string info);

		void Exception(Exception e);
	}
}
