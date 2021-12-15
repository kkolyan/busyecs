using System;
using System.Reflection;
using Kk.BusyEcs.Internal;

namespace Kk.BusyEcs
{
    public class EcsMetadata
    {
        internal readonly Type ecsContainerClass;

        private EcsMetadata(Type ecsContainerClass)
        {
            this.ecsContainerClass = ecsContainerClass;
        }

        public static EcsMetadata ScanProject()
        {
            Type ecsContainerImpl = ResolveEcsContainerClass(BusyEcs.GetUserAssemblies());
            return new EcsMetadata(ecsContainerImpl);
        }

        private static Type ResolveEcsContainerClass(Assembly[] userAssemblies)
        {
            Type ecsContainerClass = null;
            foreach (Assembly assembly in userAssemblies)
            {
                ecsContainerClass = assembly.GetType(CodeGenConstants.GeneratedEcsContainerClassName);
                if (ecsContainerClass != null)
                {
                    break;
                }
            }
            
#if UNITY_EDITOR
            if (ecsContainerClass == null)
            {
                ecsContainerClass = RuntimeEcsContainerGenerator.GenerateEcsContainer(userAssemblies);
            }
#endif
            if (ecsContainerClass == null)
            {
                throw new Exception("invalid BusyECS configuration");
            }

            return ecsContainerClass;
        }
    }
}