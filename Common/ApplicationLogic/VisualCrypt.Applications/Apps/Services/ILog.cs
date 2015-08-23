using System;

namespace VisualCrypt.Applications.Portable.Apps.Services
{
	public interface ILog
	{
		void Debug(string info);

		void Exception(Exception e);
	}
}
