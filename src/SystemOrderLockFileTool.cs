using System.IO;
using System.Reflection;
using Kk.BusyEcs.Internal;
using UnityEditor;

#if UNITY_EDITOR
namespace Kk.BusyEcs
{
    public static class SystemOrderLockFileTool
    {
        [MenuItem("Busy ECS/Generate lock file (experimental)")]
        private static void GenerateLockFile()
        {
            string lockFile = BusyEcs.SystemOrderLockFile;
            if (lockFile != null)
            {
                string s = "";
                EcsContainerSourcesGenerator.Result result = EcsContainerSourcesGenerator.GenerateEcsContainer(BusyEcs.GetUserAssemblies());
                foreach (EcsContainerSourcesGenerator.Phase phase in result.phases)
                {
                    s += $"- {phase.phaseAttribute.Name}\n";
                    foreach (MethodInfo system in phase.systems)
                    {
                        s += $"  - {system.DeclaringType.Name}.{system.Name}\n";
                    }
                }

                File.WriteAllText(lockFile, s);
            }
        }
    }
}
#endif