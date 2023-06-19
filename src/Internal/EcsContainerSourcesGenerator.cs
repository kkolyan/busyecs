#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Kk.BusyEcs.Internal
{
    public static class EcsContainerSourcesGenerator
    {
        // these constants should be in sync with the parameters of `gentool.html` used to generate BusyEcs classes
        private const int NewEntityMaxComponentCount = 8;
        private const int QueryMaxComponentCount = 4;

        public class Phase
        {
            public readonly Type phaseAttribute;
            public readonly List<MethodInfo> systems;

            public Phase(Type phaseAttribute, List<MethodInfo> systems)
            {
                this.phaseAttribute = phaseAttribute;
                this.systems = systems;
            }
        }

        public class Result
        {
            public readonly string source;
            public readonly List<Phase> phases;

            public Result(string source, List<Phase> phases)
            {
                this.source = source;
                this.phases = phases;
            }
        }

        public static Result GenerateEcsContainer(IEnumerable<Assembly> assembliesToScan)
        {
            CodegenContext context = AssemblyContextFactory.ScanAssemblies(assembliesToScan);

            ;
            return new Result(
                GenerateBody(context),
                context.phases.Select(phase => new Phase(phase, context.systemsByPhase[phase])).ToList()
            );
        }

        internal static string WorldVar(string world)
        {
            if (world == "")
            {
                return "defaultWorld";
            }

            return world;
        }

        internal static string FilterName(string world, List<Type> components) => "filter_" + WorldVar(world) + "_" + string.Join("_", components.Select(it => it.Name).OrderBy(x => x));
        internal static string FilterName(string world, List<string> components) => "filter_" + WorldVar(world) + "_" + string.Join("_", components.Select(it => it.ToString()).OrderBy(x => x));

        internal static string PoolVar(string world, Type type) => "pool_" + WorldVar(world) + "_" + type.Name;
        internal static string PoolVar(string world, string type) => "pool_" + WorldVar(world) + "_" + type;

        internal static string PhaseName(Type key)
        {
            return $"Phase_{key.Name}";
        }

        private static string GenerateBody(CodegenContext ctx)
        {
            string s = "";
            s += "using System;\n";
            s += "using System.Linq;\n";
            s += "using System.Collections.Generic;\n";
            s += "using Kk.BusyEcs;\n";
            s += "using Kk.BusyEcs.Internal;\n";
            s += "using Leopotam.EcsLite;\n";
            s += "using System.Reflection;\n";
            s += "[UnityEngine.Scripting.Preserve]\n";
            s += "public class " + CodeGenConstants.GeneratedEcsContainerClassName + " : Kk.BusyEcs.Internal.IConfigurableEcsContainer {\n";
            s += GenerateFields(ctx);
            s += "  public " + CodeGenConstants.GeneratedEcsContainerClassName + "() {\n";
            s += GenerateConstructor(ctx);
            s += "  }\n";
            s += "  public void Init(EcsSystems worlds) {\n";
            s += GenerateInit(ctx);
            s += "  }\n";
            s += "  public void Execute<T>() where T : Attribute {\n";
            s += "    _phaseExecutionByType[typeof(T)]();\n";
            s += "  }\n";
            s += GenerateMethods(ctx);
            s += GenerateNestedClasses(ctx);
            s += "}\n";
            return s;
        }

        private static string GenerateFields(CodegenContext ctx)
        {
            string s = "";

            s += "  private readonly Dictionary<Type, Action> _phaseExecutionByType = new Dictionary<Type, Action>();\n";
            s += "  private EcsSystems worlds;\n";
            s += "  private Dictionary<Type, object> injectables = new Dictionary<Type, object>();\n";
            s += "  public EcsWorld[] allWorlds;\n";

            foreach (var systemClass in ctx.systemClasses)
            {
                s += "  private " + systemClass.FullName + " " + SystemInstanceVar(systemClass) + ";\n";
            }

            foreach (string world in ctx.worlds)
            {
                s += "  private EcsWorld " + WorldVar(world) + ";\n";
                foreach (var componentType in ctx.components)
                {
                    s += "  public EcsPool<" + componentType.FullName + "> " + PoolVar(world, componentType) + ";\n";
                }

                foreach (var pair in ctx.filters.Values)
                {
                    s += "  public EcsFilter " + FilterName(world, pair) + ";\n";
                }
            }

            return s;
        }

        private static string SystemInstanceVar(Type type)
        {
            return "_" + type.Name;
        }

        private static string GenerateInit(CodegenContext ctx)
        {
            string s = "";
            s += "    this.worlds = worlds;\n";
            foreach (var systemClass in ctx.systemClasses)
            {
                s += "    " + SystemInstanceVar(systemClass) + " = new " + systemClass.FullName + " ();\n";
            }

            s += $"    allWorlds = new EcsWorld[{ctx.worlds.Count}];\n";
            for (var worldIndex = 0; worldIndex < ctx.worlds.Count; worldIndex++)
            {
                string world = ctx.worlds[worldIndex];
                if (world == "")
                {
                    s += "    " + WorldVar(world) + " = worlds.GetWorld();\n";
                }
                else
                {
                    s += "    if (worlds.GetWorld(\"" + world + "\") == null) { worlds.AddWorld(new EcsWorld(), \"" + world + "\"); }\n";
                    s += "    " + WorldVar(world) + " = worlds.GetWorld(\"" + world + "\");\n";
                }

                s += $"    allWorlds[{worldIndex}] = " + WorldVar(world) + ";\n";

                foreach (var componentType in ctx.components)
                {
                    s += "    " + PoolVar(world, componentType) + " = " + WorldVar(world) + ".GetPool<" + componentType.FullName + ">();\n";
                }

                foreach (var pair in ctx.filters.Values)
                {
                    s += "    " + FilterName(world, pair) + " = " + WorldVar(world) + ".Filter<" + pair[0].FullName + ">()";
                    for (var i = 1; i < pair.Count; i++)
                    {
                        s += ".Inc<" + pair[i] + ">()";
                    }

                    s += ".End();\n";
                }
            }

            s += $"    AddInjectable(this, typeof({nameof(IEnv)}));\n";
            s += $"    AddInjectable(this, typeof({CodeGenConstants.GeneratedEcsContainerClassName}));\n";
            s += "    typeof(Entity).GetField(\"env\", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, this);\n";
            s += "    WorldsKeeper.worlds = allWorlds.ToArray();\n";

            foreach (CodegenContext.Injection injection in ctx.injections)
            {
                s += "    " + SystemInstanceVar(injection.target) + "." + injection.field + " = (" + injection.subject.FullName +
                     ") ResolveInjectable<" +
                     injection.subject.FullName + ">();\n";
            }

            return s;
        }

        private static string GenerateConstructor(CodegenContext ctx)
        {
            string s = "";
            foreach (var pair in ctx.systemsByPhase)
            {
                s += $"    _phaseExecutionByType[typeof({pair.Key.FullName})] = {PhaseName(pair.Key)};\n";
            }
            return s;
        }


        private static string GenerateMethods(CodegenContext ctx)
        {
            string s = "";
            s += "  public void AddInjectable(object injectable, Type overrideType = null)\n";
            s += "  {\n";
            s += "    injectables[overrideType ?? injectable.GetType()] = injectable;\n";
            s += "  }\n";
            s += "  private object ResolveInjectable<T>()\n";
            s += "  {\n";
            s += "    if (!injectables.TryGetValue(typeof(T), out var injectable))\n";
            s += "    {\n";
            s += "        throw new Exception(\"failed to resolve injection of \" + typeof(T).FullName);\n";
            s += "    }\n";
            s += "    \n";
            s += "    return injectable;\n";
            s += "  }\n";
            
            s += GeneratePhaseMethods(ctx);

            for (int componentCount = 1; componentCount <= NewEntityMaxComponentCount; componentCount++)
            {
                string gsig = "";

                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) gsig += ", ";

                    gsig += "T" + i;
                }

                s += "  public Entity NewEntity<" + gsig + ">(";

                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ", ";
                    s += "in T" + i + " c" + i;
                    // s += "T" + i + " c" + i;
                }

                s += ") \n";

                for (int i = 1; i <= componentCount; i++)
                {
                    s += "    where T" + i + " : struct \n";
                }


                s += "  {\n";
                s += "    EcsWorld w = worlds.GetWorld(WorldName<" + gsig + ">.worldName);\n";
                s += "    var id = w.NewEntity();\n";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"    w.GetPool<T{i}>().Add(id) = c{i};\n";
                }

                s += "    return new Entity(w, id);\n";
                s += "  }\n";
            }

            for (int componentCount = 1; componentCount <= QueryMaxComponentCount; componentCount++)
            {
                for (int rd = 0; rd <= componentCount; rd++)
                {
                    string gsig = "";
                    string where = "";

                    for (int i = 1; i <= componentCount; i++)
                    {
                        if (i > 1) gsig += ", ";

                        gsig += "T" + i;
                    }

                    for (int i = 1; i <= componentCount; i++)
                    {
                        where += "    where T" + i + " : struct \n";
                    }

                    s += $"  public void Query<{gsig}>(SimpleCallback{rd}<{gsig}> callback)\n{where}  {{\n";
                    s += "    for (int wi = 0; wi < allWorlds.Length; wi++) {\n";
                    s += "      EcsWorld w = allWorlds[wi];\n";

                    s += "      EcsFilter filter = w.Filter<T1>()";
                    for (int i = 2; i <= componentCount; i++)
                    {
                        s += ".Inc<T" + i + ">()";
                    }

                    s += ".End();\n";
                    s += "      foreach (var id in filter) {\n";
                    if (!BusyEcs.SkipIterationCheck)
                    {
                        for (int i = 1; i <= componentCount; i++)
                        {
                            s += $"        if (!PoolKeeper<T{i}>.byWorld[wi].Has(id)) continue;\n";
                        }
                    }

                    s += "        callback(";
                    for (int i = 1; i <= componentCount; i++)
                    {
                        if (i > 1) s += ", ";
                        s += i <= rd ? "ref " : "";
                        s += $"PoolKeeper<T{i}>.byWorld[wi].Get(id)";
                    }

                    s += ");\n";
                    s += "      }\n";

                    s += "    }\n";
                    s += "  }\n";
                    s += $"  public void Query<{gsig}>(EntityCallback{rd}<{gsig}> callback)\n{@where}  {{\n";
                    s += "    for (int wi = 0; wi < allWorlds.Length; wi++) {\n";
                    s += "      EcsWorld w = allWorlds[wi];\n";

                    s += "      EcsFilter filter = w.Filter<T1>()";
                    for (int i = 2; i <= componentCount; i++)
                    {
                        s += ".Inc<T" + i + ">()";
                    }

                    s += ".End();\n";
                    s += "      foreach (var id in filter) {\n";
                    if (!BusyEcs.SkipIterationCheck)
                    {
                        for (int i = 1; i <= componentCount; i++)
                        {
                            s += $"        if (!PoolKeeper<T{i}>.byWorld[wi].Has(id)) continue;\n";
                        }
                    }

                    s += "        callback(new Entity(wi, id), ";
                    for (int i = 1; i <= componentCount; i++)
                    {
                        if (i > 1) s += ", ";
                        s += i <= rd ? "ref " : "";
                        s += $"PoolKeeper<T{i}>.byWorld[wi].Get(id)";
                    }

                    s += ");\n";
                    s += "      }\n";

                    s += "    }\n";
                    s += "  }\n";
                }
            }

            for (int componentCount = 1; componentCount <= QueryMaxComponentCount; componentCount++)
            {
                string gsig = "";
                string where = "";

                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) gsig += ", ";

                    gsig += "T" + i;
                }

                for (int i = 1; i <= componentCount; i++)
                {
                    where += "    where T" + i + " : struct \n";
                }

                s += $"  public bool Match<{gsig}>(Entity entity";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $", out Ref<T{i}> c{i}";
                }
                s += $")\n{@where} {{\n";
                s += "    if (";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1)
                    {
                        s += " || ";
                    }
                    s += $"!entity.Has<T{i}>()";
                }

                s += ") {\n";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"      c{i} = default;\n";
                }
                s += "      return false;\n";
                s += "    }\n";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"    c{i} = new Ref<T{i}>(entity);\n";
                }
                s += "    return true;\n";
                s += "  }\n";
                
                for (int rd = 0; rd <= componentCount; rd++)
                {
                    s += $"  public bool Match<{gsig}>(Entity entity, SimpleCallback{rd}<{gsig}> callback)\n{@where}  {{\n";
                    for (int i = 1; i <= componentCount; i++)
                    {
                        s += $"    if (!entity.Has<T{i}>()) return false;\n";
                    }

                    s += "    callback(";
                    for (int i = 1; i <= componentCount; i++)
                    {
                        if (i > 1) s += ", ";
                        s += i <= rd ? "ref " : "";
                        s += $"entity.Get<T{i}>()";
                    }

                    s += ");\n";
                    s += "    return true;\n";

                    s += "  }\n";
                    

                    s += $"  public bool Match<{gsig}>(Entity entity, EntityCallback{rd}<{gsig}> callback)\n{@where}  {{\n";
                    for (int i = 1; i <= componentCount; i++)
                    {
                        s += $"    if (!entity.Has<T{i}>()) return false;\n";
                    }

                    s += "    callback(entity, ";
                    for (int i = 1; i <= componentCount; i++)
                    {
                        if (i > 1) s += ", ";
                        s += i <= rd ? "ref " : "";
                        s += $"entity.Get<T{i}>()";
                    }

                    s += ");\n";
                    s += "    return true;\n";

                    s += "  }\n";
                }
            }


            return s;
        }

        private static string GeneratePhaseMethods(CodegenContext ctx)
        {
            string s = "";
            foreach (var pair in ctx.systemsByPhase)
            {
                s += $"  private void {PhaseName(pair.Key)}() {{\n";
                foreach (MethodInfo method in pair.Value)
                {
                    if (method.GetParameters().Length <= 0)
                    {
                        s += "    " + SystemInstanceVar(method.DeclaringType) + "." + method.Name + "();\n";
                    }
                    else
                    {
                        bool supplyEntity = false;
                        List<Type> components = new List<Type>();
                        foreach (ParameterInfo parameter in method.GetParameters())
                        {
                            if (IsEntity(parameter.ParameterType))
                            {
                                supplyEntity = true;
                            }
                            else
                            {
                                if (parameter.ParameterType.IsByRef)
                                {
                                    components.Add(parameter.ParameterType.GetElementType());
                                }
                                else
                                {
                                    components.Add(parameter.ParameterType);
                                }
                            }
                        }

                        for (var worldIndex = 0; worldIndex < ctx.worlds.Count; worldIndex++)
                        {
                            string world = ctx.worlds[worldIndex];
                            s += $"    foreach (var entity in {FilterName(world, components)}) {{\n";
                            s += $"      {SystemInstanceVar(method.DeclaringType)}.{method.Name}(";
                            if (supplyEntity) { }

                            bool first = true;
                            foreach (ParameterInfo parameter in method.GetParameters())
                            {
                                if (first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    s += ", ";
                                }

                                if (IsEntity(parameter.ParameterType))
                                {
                                    s += $"new Entity({worldIndex}, entity)";
                                }
                                else
                                {
                                    Type componentType;
                                    if (parameter.ParameterType.IsByRef)
                                    {
                                        if (!parameter.IsIn && !parameter.IsOut)
                                        {
                                            s += "ref ";
                                        }

                                        componentType = parameter.ParameterType.GetElementType();
                                    }
                                    else
                                    {
                                        componentType = parameter.ParameterType;
                                    }

                                    s += $"{PoolVar(world, componentType)}.Get(entity)";
                                }
                            }

                            s += ");\n";
                            s += "    }\n";
                        }
                    }
                }

                s += "  }\n";
            }

            return s;
        }

        internal static bool IsEntity(Type type)
        {
            return type == typeof(Entity) || type.IsByRef && type.GetElementType() == typeof(Entity);
        }

        private static string GenerateNestedClasses(CodegenContext ctx)
        {
            var s = "";

            for (int componentCount = 1; componentCount <= NewEntityMaxComponentCount; componentCount++)
            {
                s += "  private static class WorldName<";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ", ";

                    s += "T" + i;
                }

                s += "> {\n";
                s += "    internal static readonly string worldName = Kk.BusyEcs.Internal.WorldResolve.ResolveWorldName(";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ", ";

                    s += "typeof(T" + i + ")";
                }

                s += ");\n";
                s += "  }\n";
            }

            return s;
        }
    }
}
#endif