using System;
using System.Collections.Generic;

namespace VisualCrypt.Applications.Services.Interfaces
{
    public static class Service
    {
        readonly static Dictionary<string, Implementation> Container = new Dictionary<string, Implementation>();

        public static void Register<TInterface, TImplementation>(bool isInstanceShared, string instanceLabel = null, bool replaceExisting = false)
        {
            if (replaceExisting)
                Container.Remove(GetKey<TInterface>(instanceLabel));

            Container.Add(GetKey<TInterface>(instanceLabel), new Implementation
            {
                ImplementationType = typeof(TImplementation),
                Instance = null,
                IsInstanceShared = isInstanceShared
            });
        }


        static string GetKey<TInterface>(string instanceLabel)
        {
            if (instanceLabel == null)
                return typeof (TInterface).FullName;
            return string.Format("{0} - {1}", typeof (TInterface).FullName, instanceLabel);
        }

        public static TInterface Get<TInterface>(string instanceLabel = null)
        {
            var key = GetKey<TInterface>(instanceLabel);
            if (!Container.ContainsKey(key))
                throw new InvalidOperationException(string.Format("{0} is not available from the container.", typeof(TInterface)));

            var typeInstance = Container[key];

            if (!typeInstance.IsInstanceShared)
                return (TInterface)Activator.CreateInstance(typeInstance.ImplementationType);

            if (typeInstance.Instance != null)
                return (TInterface)typeInstance.Instance;

            typeInstance.Instance = Activator.CreateInstance(typeInstance.ImplementationType);
            return (TInterface)typeInstance.Instance;
        }

        sealed class Implementation
        {
            public Type ImplementationType;
            public object Instance;
            public bool IsInstanceShared;
        }

        
    }
}
