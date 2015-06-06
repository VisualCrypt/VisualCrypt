using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace VisualCrypt.Desktop.Shared.App
{
	[Export(typeof(IParamsProvider))]
	public class ParamsProvider : IParamsProvider
	{
		static readonly Dictionary<Type, object> OptionsByType = new Dictionary<Type, object>();

		/// <summary>
		/// Allows for the injection of parameters for objects composed by MEF.
		/// The parameter object is stored in a static readonly Dictionary&lt;Type, object&gt;
		/// and deleted after the next call to GetParams().
		/// </summary>
		public static void SetParams<T>(Type windowType, T options)
		{
			if (OptionsByType.ContainsKey(windowType))
				OptionsByType[windowType] = options;
			else
				OptionsByType.Add(windowType, options);
		}


		public TParams GetParams<TKey, TParams>()
		{
			var key = typeof(TKey);
			if (OptionsByType.ContainsKey(key))
			{
				var instance = (TParams)OptionsByType[key];
				OptionsByType.Remove(key);
				return instance;

			}
			throw new InvalidOperationException("ParamsProvider: no options found, did you set them with ParamsProvider.SetParams()?");
		}
	}

	public interface IParamsProvider
	{
		TParams GetParams<TKey, TParams>();
	}
}
