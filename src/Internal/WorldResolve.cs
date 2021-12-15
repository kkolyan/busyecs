using System;
using System.Reflection;

namespace Kk.BusyEcs.Internal
{
    /// <summary>
    /// used by generated user code
    /// <see cref="CodeGenConstants.GeneratedEcsContainerClassName"/> 
    /// </summary>
    public static class WorldResolve
    {
        public static string ResolveWorldName(params Type[] componentTypes)
        {
            string worldName = null;
            foreach (Type componentType in componentTypes)
            {
                string w = componentType.GetCustomAttribute<EcsWorldAttribute>()?.name;
                if (worldName != null && worldName != w)
                {
                    throw new Exception($"world resolution conflict: {worldName} and {w}");
                }

                if (w != "") // attribute value cannot be null
                {
                    worldName = w;
                }
            }

            return worldName;
        }
    }
}