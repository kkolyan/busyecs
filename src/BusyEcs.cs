using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kk.BusyEcs
{
    public static class BusyEcs
    {
        
        private static Assembly[] _userAssemblies;
        public static string SystemOrderLockFile { get; set; }
        
        public delegate void SortSystems(MethodInfo[] systems);
        public static SortSystems SystemsOrder { get; set; }

        public static void SetUserAssemblies(IEnumerable<Assembly> userAssemblies)
        {
            Assembly[] array = userAssemblies.ToArray();
            if (array.Length <= 0)
            {
                throw new Exception(
                    "at least one user assembly required. the easiest solution is to add following to builder: " +
                    "\"new EcsContainerFactory(typeof(AnyOfMyMonoBehaviors).Assembly)\"");
            }
            _userAssemblies = array;
        }

        public static void SetUserAssemblies(Assembly userAssembly0, params Assembly[] userAssembliesN)
        {
            SetUserAssemblies(Enumerable.Empty<Assembly>()
                .Append(userAssembly0)
                .Concat(userAssembliesN));
        }

        internal static Assembly[] GetUserAssemblies()
        {
            if (_userAssemblies == null)
            {
                throw new Exception("BusyECS should be initialized with user assemblies");
            }

            return _userAssemblies;
        }
    }
}