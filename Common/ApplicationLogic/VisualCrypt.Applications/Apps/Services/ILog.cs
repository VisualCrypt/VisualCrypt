using System;

namespace VisualCrypt.Applications.Apps.Services
{
	public interface ILog
	{
		void Debug(string info);

		void Exception(Exception e);
	}
}
