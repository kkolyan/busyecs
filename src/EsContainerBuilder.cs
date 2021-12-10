using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public class EsContainerBuilder
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private List<Assembly> _assemblies = new List<Assembly>();
        private EcsSystems _ecsSystems;

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

        // for LeoECS Lite plugins integration. BusyECS uses it ONLY as worlds container (invokes GetAllNamedWorlds, GetWorld, AddWorld).
        public EsContainerBuilder Integrate(EcsSystems ecsSystems)
        {
            _ecsSystems = ecsSystems;
            return this;
        }

        public IEcsContainer End()
        {
            return new NaiveEcsContainer(_services, _assemblies.Distinct().ToList(), _ecsSystems);
        }
    }
}