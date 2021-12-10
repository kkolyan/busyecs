using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kk.BusyEcs
{
    public class EsContainerBuilder
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private List<Assembly> _assemblies = new List<Assembly>();

        public EsContainerBuilder Injectable(object service, Type overrideType = null)
        {
            _services[overrideType ?? service.GetType()] = service;
            return this;
        }

        public EsContainerBuilder ScanAll()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Scan(assembly);
            }

            return this;
        }

        public EsContainerBuilder Scan(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        public IEcsContainer End()
        {
            return new NaiveEcsContainer(_services, _assemblies.Distinct().ToList());
        }
    }
}