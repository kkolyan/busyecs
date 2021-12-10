using System;
using System.Collections.Generic;
using System.Reflection;
using Leopotam.EcsLite;
using UnityEngine;

namespace Kk.BusyEcs
{
    public class EcsContainerBuilder
    {
        private readonly Dictionary<Type, string> _worldRequirements = new Dictionary<Type, string>();
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private EcsSystems _ecsSystems;
        private List<Type> _systemClasses = new List<Type>();

        public EcsContainerBuilder Injectable(object service, Type overrideType = null)
        {
            _services[overrideType ?? service.GetType()] = service;
            return this;
        }

        public EcsContainerBuilder ScanAll()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Scan(assembly);
            }

            return this;
        }

        public EcsContainerBuilder Scan(Assembly assembly)
        {
            Debug.Log($"Scanning assembly: {assembly}");
            return ScanTypes(assembly.GetTypes());
        }

        public EcsContainerBuilder ScanTypes(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                ScanType(type);
            }

            return this;
        }

        public EcsContainerBuilder ScanType(Type type)
        {
            if (type.GetCustomAttribute<EcsSystemAttribute>() != null)
            {
                _systemClasses.Add(type);
            }

            EcsWorldAttribute ecsWorldAttribute = type.GetCustomAttribute<EcsWorldAttribute>();
            if (ecsWorldAttribute != null)
            {
                _worldRequirements[type] = ecsWorldAttribute.name;
            }

            return this;
        }

        // for LeoECS Lite plugins integration. BusyECS uses it ONLY as worlds container (invokes GetAllNamedWorlds, GetWorld, AddWorld).
        public EcsContainerBuilder Integrate(EcsSystems ecsSystems)
        {
            _ecsSystems = ecsSystems;
            return this;
        }

        public IEcsContainer End()
        {
            return new NaiveEcsContainer(_services, _systemClasses, _ecsSystems, _worldRequirements);
        }
    }
}