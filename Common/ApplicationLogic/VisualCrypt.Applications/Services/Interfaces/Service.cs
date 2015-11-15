using System;
using System.Collections.Generic;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public static class Service
    {
        static readonly Dictionary<string, Implementation> Container = new Dictionary<string, Implementation>();

        public static void Register<TKey, TImplementation>(bool isInstanceShared, string instanceLabel = null, bool replaceExisting = false) where TImplementation : class
        {
            if (replaceExisting)
                Container.Remove(GetKey<TKey>(instanceLabel));

            Container.Add(GetKey<TKey>(instanceLabel), new Implementation
            {
                ImplementationType = typeof(TImplementation),
                Instance = null,
                IsInstanceShared = isInstanceShared
            });
        }


        static string GetKey<TKey>(string instanceLabel)
        {
            return instanceLabel == null 
                ? typeof(TKey).FullName 
                : $"{typeof (TKey).FullName} - {instanceLabel}";
        }

        public static TKey Get<TKey>(string instanceLabel = null) where TKey :class
        {
            var key = GetKey<TKey>(instanceLabel);
            if (!Container.ContainsKey(key))
                return null;

            var typeInstance = Container[key];

            if (!typeInstance.IsInstanceShared)
                return (TKey)Activator.CreateInstance(typeInstance.ImplementationType);

            if (typeInstance.Instance != null)
                return (TKey)typeInstance.Instance;

            typeInstance.Instance = Activator.CreateInstance(typeInstance.ImplementationType);
            return (TKey)typeInstance.Instance;
        }


        sealed class Implementation
        {
            public Type ImplementationType;
            public object Instance;
            public bool IsInstanceShared;
        }
    }
}
