#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Kk.BusyEcs.Internal
{
    internal static class EditorTools
    {
        private const string CodegenFile = "Assets/BusyECS.generated.cs";

        internal static bool CodegenDebug
        {
            get => EditorPrefs.GetBool("BusyECS.codegenDebugMode");
            set => EditorPrefs.SetBool("BusyECS.codegenDebugMode", value);
        }

        [InitializeOnLoadMethod]
        public static void EnsureFile()
        {
            if (!CodegenDebug)
            {
                TruncateGeneratedCode();
            }
        }

        internal static void TruncateGeneratedCode()
        {
            File.WriteAllText(CodegenFile,
                "// used as temporary storage by BusyECS at build time.there is no sense to change it.\n" +
                "// will be removed after integration to build and runtime codegen");
        }

        internal static void GenerateCodeToFile() { File.WriteAllText(CodegenFile, GenerateCode()); }

        private static string GenerateCode()
        {
            EcsContainerSourcesGenerator.Result result = EcsContainerSourcesGenerator.GenerateEcsContainer(BusyEcs.GetUserAssemblies());
            return result.source;
        }

        internal static void ExportGeneratedCode()
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

        [MenuItem("Busy ECS/Debug...")]
        private static void OpenMe()
        {
            EditorWindow.GetWindow<CodegenDebugWindow>("Busy ECS debug");
        }

        internal static void DumpSystemsOrder(string file)
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

            File.WriteAllText(file, s);
        }
    }

    internal class CodegenDebugWindow : EditorWindow
    {
        private void OnGUI()
        {
            bool codegenDebug = EditorGUILayout.ToggleLeft("Debug Mode", EditorTools.CodegenDebug);
            if (codegenDebug != EditorTools.CodegenDebug)
            {
                EditorTools.CodegenDebug = codegenDebug;
                if (codegenDebug)
                {
                    EditorTools.GenerateCodeToFile();
                }
                else
                {
                    EditorTools.TruncateGeneratedCode();
                }
            }

            using (new EditorGUI.DisabledScope(!EditorTools.CodegenDebug))
            {
                if (GUILayout.Button("Generate code"))
                {
                    EditorTools.GenerateCodeToFile();
                }
            }

            if (GUILayout.Button("Cleanup"))
            {
                EditorTools.TruncateGeneratedCode();
            }

            if (GUILayout.Button("Export generated code..."))
            {
                EditorTools.ExportGeneratedCode();
            }

            if (GUILayout.Button("Generate optimized systems..."))
            {
                SystemsOptimizer.GenerateOptimizedSystems(BusyEcs.GetUserAssemblies());
            }

            string lockFile = BusyEcs.SystemOrderDumpFile;
            if (lockFile != null)
            {
                if (GUILayout.Button("Dump systems order"))
                {
                    EditorTools.DumpSystemsOrder(lockFile);
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    GUILayout.Button(new GUIContent("Dump systems order") {tooltip = $"set {nameof(BusyEcs)}.{nameof(BusyEcs.SystemOrderDumpFile)} to use it"});
                }
            }
        }
    }

    internal class BusyEcsBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPostprocessBuild(BuildReport report)
        {
            EditorTools.EnsureFile();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            EditorTools.GenerateCodeToFile();
        }
    }
}
#endif