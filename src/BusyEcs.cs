using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kk.BusyEcs
{
    public static class BusyEcs
    {
        private static Assembly[] _userAssemblies;
        private static string _systemOrderLockFile;

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

        public static void SetSystemOrderLockFile(string lockFile)
        {
            _systemOrderLockFile = lockFile;
        }

        public static string GetSystemOrderLockFile()
        {
            return _systemOrderLockFile;
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