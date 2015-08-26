using System;
using System.Collections.Generic;
using VisualCrypt.Applications.Services.Interfaces;

namespace VisualCrypt.Applications.Services.PortableImplementations
{
	public class ParamsProvider : IParamsProvider
	{
		static readonly Dictionary<Type, object> OptionsByType = new Dictionary<Type, object>();

		/// <summary>
		/// Allows passing parameters to objects that have only a default constructor.
		/// The parameter object is stored in a static readonly Dictionary&lt;Type, object&gt;
		/// and deleted after the next call to GetParams().
		/// </summary>
		/// <param name="key">The Type name where the parameters are used should be used as key.</param>
		/// <param name="parameterObject">The parameter object for use at the target.</param>
		public void SetParams<T>(Type key, T parameterObject)
		{
			if (OptionsByType.ContainsKey(key))
				OptionsByType[key] = parameterObject;
			else
				OptionsByType.Add(key, parameterObject);
		}

        ///<summary>
        /// Retrieves the parameter object that was passed with SetParams.
        /// </summary>
        /// <typeparam name="TKey">The Type name where the parameters are used should be used as key.</typeparam>
        /// <typeparam name="TParams">The parameter object for use at the target.</typeparam>
		public TParams GetParams<TKey, TParams>()
		{
			var key = typeof (TKey);
			if (OptionsByType.ContainsKey(key))
			{
				var instance = (TParams) OptionsByType[key];
				OptionsByType.Remove(key);
				return instance;
			}
			throw new InvalidOperationException(
				"ParamsProvider: no paramematerObject found, did you set them with ParamsProvider.SetParams()?");
		}
	}

	
}