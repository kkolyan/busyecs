#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using Kk.BusyEcs.Internal;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Kk.BusyEcs
{
    public sealed class BusyEcsBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string CodegenFile = "Assets/BusyECS.generated.cs";

        [MenuItem("Busy ECS/Export generated code...")]
        public static void ExportGeneratedCode()
        {
            string exportAs = EditorUtility.SaveFilePanel("Export generated code...", "", "", "cs");
            if (!string.IsNullOrEmpty(exportAs))
            {
                File.WriteAllText(exportAs, GenerateCode());
                int option = EditorUtility.DisplayDialogComplex("Generated code exported", "", "Open file", "Close", "Open file location");
                if (option == 0)
                {
                    Process.Start(exportAs);
                }
                else if (option == 2)
                {
                    Process.Start(Path.GetDirectoryName(exportAs) ?? throw new Exception("WTF"));
                }
            }
        }

        [InitializeOnLoadMethod]
        [MenuItem("Busy ECS/Reset codegen stubs")]
        private static void EnsureFile()
        {
            File.WriteAllText(CodegenFile,
                "// used as temporary storage by BusyECS at build time.there is no sense to change it.\n" +
                "// will be removed after integration to build and runtime codegen");
        }

        public int callbackOrder { get; }


        public void OnPostprocessBuild(BuildReport report)
        {
            EnsureFile();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            File.WriteAllText(CodegenFile, GenerateCode());
        }

        private static string GenerateCode()
        {
            EcsContainerSourcesGenerator.Result result = EcsContainerSourcesGenerator.GenerateEcsContainer(BusyEcs.GetUserAssemblies());
            string source = result.source;
            return source;
        }
    }
}
#endif