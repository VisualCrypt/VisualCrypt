using System;
using System.Linq;
using System.Reflection;

namespace VisualCrypt.Desktop
{
	/// <summary>
	/// Copies the source's properties to the 
	/// destination object where the types match.
	/// </summary>
	public static class Map
	{
		public static void Copy(object source, object destination)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (destination == null)
				throw new ArgumentNullException("destination");

			PropertyInfo[] sourceProperties = source.GetType().GetProperties();
			PropertyInfo[] destinationProperties = destination.GetType().GetProperties();

			var commonProperties = from sp in sourceProperties
				join dp in destinationProperties
					on new {sp.Name, sp.PropertyType}
					equals
					new {dp.Name, dp.PropertyType}
				select new {sp, dp};

			foreach (var match in commonProperties)
			{
				match.dp.SetValue(destination, match.sp.GetValue(source, null), null);
			}
		}
	}
}