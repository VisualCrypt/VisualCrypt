using System;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public interface IParamsProvider
    {
        /// <summary>
		/// Allows passing parameters to objects that have only a default constructor.
		/// The parameter object is stored in a static readonly Dictionary&lt;Type, object&gt;
		/// and deleted after the next call to GetParams().
		/// </summary>
		/// <param name="key">The Type name where the parameters are used should be used as key.</param>
		/// <param name="parameterObject">The parameter object for use at the target.</param>
        void SetParams<T>(Type key, T parameterObject);

        ///<summary>
        /// Retrieves the parameter object that was passed with SetParams.
        /// </summary>
        /// <typeparam name="TKey">The Type name where the parameters are used should be used as key.</typeparam>
        /// <typeparam name="TParams">The parameter object for use at the target.</typeparam>
        TParams GetParams<TKey, TParams>();
    }
}
