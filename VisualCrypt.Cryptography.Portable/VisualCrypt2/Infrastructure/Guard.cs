using System;
using System.Runtime.CompilerServices;

namespace VisualCrypt.Cryptography.Portable.VisualCrypt2.Infrastructure
{
	public static class Guard
	{
		public static void NotNull(object[] args, [CallerMemberName] string method = "")
		{
			for (var i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					throw new ArgumentNullException(string.Format("Parameter at index {0} of method {1}", i, method));
				}
			}
		}
	}
}
