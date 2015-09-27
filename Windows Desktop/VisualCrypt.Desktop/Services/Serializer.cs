using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace VisualCrypt.Desktop.Services
{
	public static class Serializer<T>
	{
		public static T Deserialize(string data)
		{
			if (string.IsNullOrEmpty(data))
				return default(T);

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));

			var ser = new DataContractSerializer(typeof (T));
			var options = (T) ser.ReadObject(stream);
			return options;
		}

		public static T Deserialize(string data, IEnumerable<Type> knownTypes)
		{
			if (string.IsNullOrEmpty(data))
				return default(T);

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));

			var ser = new DataContractSerializer(typeof (T), knownTypes);
			var options = (T) ser.ReadObject(stream);
			return options;
		}

		public static string Serialize(T profile)
		{
			var stream = new MemoryStream();
			var ser = new DataContractSerializer(typeof (T));
			ser.WriteObject(stream, profile);
            var data = stream.ToArray();
            var serialized = Encoding.UTF8.GetString(data, 0, data.Length);
            return serialized;
		}

		public static string Serialize(T profile, IEnumerable<Type> knownTypes)
		{
			var stream = new MemoryStream();
			var ser = new DataContractSerializer(typeof (T), knownTypes);
			ser.WriteObject(stream, profile);
		    var data = stream.ToArray();
			var serialized = Encoding.UTF8.GetString(data,0,data.Length);
			return serialized;
		}
	}
}