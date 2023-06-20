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
            Context context = new Context();
            Scan(context, assembliesToScan);

            ;
            return new Result(
                GenerateBody(context),
                context.phases.Select(phase => new Phase(phase, context.systemsByPhase[phase])).ToList()
            );
        }

        private class Injection
        {
            public Type system;
            public Type type;
            public string field;
        }

        private class Context
        {
            public List<Injection> injections = new List<Injection>();
            public List<Type> phases = new List<Type>();
            public Dictionary<Type, List<MethodInfo>> systemsByPhase = new Dictionary<Type, List<MethodInfo>>();
            public List<Type> systemClasses = new List<Type>();
            public HashSet<Type> components = new HashSet<Type>();
            public List<List<Type>> filters = new List<List<Type>>();
            public List<string> worlds = new List<string>();
        }

        private static string GenerateBody(Context ctx)
        {
            string s = "";
            s += "using System;\n";
            s += "using System.Collections.Generic;\n";
            s += "using Kk.BusyEcs;\n";
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

        private static string GenerateFields(Context ctx)
        {
            string s = "";

            s += "  private readonly Dictionary<Type, Action> _phaseExecutionByType = new Dictionary<Type, Action>();\n";
            s += "  private EcsSystems worlds;\n";
            s += "  private Dictionary<Type, object> injectables = new Dictionary<Type, object>();\n";
            s += "  private List<EcsWorld> allWorlds = new List<EcsWorld>();\n";

            foreach (Type systemClass in ctx.systemClasses)
            {
                s += "  private " + systemClass.FullName + " " + SystemInstanceVar(systemClass) + ";\n";
            }

            foreach (string world in ctx.worlds)
            {
                s += "  private EcsWorld " + WorldVar(world) + ";\n";
                foreach (Type componentType in ctx.components)
                {
                    s += "  private EcsPool<" + componentType.FullName + "> " + PoolVar(world, componentType) + ";\n";
                }

                foreach (List<Type> pair in ctx.filters)
                {
                    s += "  private EcsFilter " + FilterName(world, pair) + ";\n";
                }
            }

            return s;
        }

        private static string SystemInstanceVar(Type type)
        {
            return "_" + type.Name;
        }

        private static string GenerateInit(Context ctx)
        {
            string s = "";
            s += "    this.worlds = worlds;\n";
            foreach (Type systemClass in ctx.systemClasses)
            {
                s += "    " + SystemInstanceVar(systemClass) + " = new " + systemClass.FullName + " ();\n";
            }

            foreach (string world in ctx.worlds)
            {
                if (world == "")
                {
                    s += "    " + WorldVar(world) + " = worlds.GetWorld();\n";
                }
                else
                {
                    s += "    if (worlds.GetWorld(\"" + world + "\") == null) { worlds.AddWorld(new EcsWorld(), \"" + world + "\"); }\n";
                    s += "    " + WorldVar(world) + " = worlds.GetWorld(\"" + world + "\");\n";
                }

                s += "    allWorlds.Add(" + WorldVar(world) + ");\n";

                foreach (Type componentType in ctx.components)
                {
                    s += "    " + PoolVar(world, componentType) + " = " + WorldVar(world) + ".GetPool<" + componentType.FullName + ">();\n";
                }

                foreach (List<Type> pair in ctx.filters)
                {
                    s += "    " + FilterName(world, pair) + " = " + WorldVar(world) + ".Filter<" + pair[0].FullName + ">()";
                    for (var i = 1; i < pair.Count; i++)
                    {
                        s += ".Inc<" + pair[i] + ">()";
                    }

                    s += ".End();\n";
                }
            }

            foreach (Injection injection in ctx.injections)
            {
                s += "    " + SystemInstanceVar(injection.system) + "." + injection.field + " = (" + injection.type.FullName +
                     ") ResolveInjectable<" +
                     injection.type.FullName + ">();\n";
            }

            return s;
        }

        private static string WorldVar(string world)
        {
            if (world == "")
            {
                return "defaultWorld";
            }

            return world;
        }

        private static string FilterName(string world, List<Type> components)
        {
            return "filter_" + WorldVar(world) + "_" + string.Join("_", components.Select(it => it.Name).OrderBy(x => x));
        }

        private static string PoolVar(string world, Type type)
        {
            return "pool_" + WorldVar(world) + "_" + type.Name;
        }

        private static string GenerateConstructor(Context ctx)
        {
            long nextVarId = 0;
            string s = "";
            s += "    AddInjectable(this, typeof(IEnv));\n";
            s += "    typeof(Entity).GetField(\"env\", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, this);\n";
            foreach (KeyValuePair<Type, List<MethodInfo>> pair in ctx.systemsByPhase)
            {
                s += "    _phaseExecutionByType[typeof(" + pair.Key.FullName + ")] = () => {\n";
                foreach (MethodInfo method in pair.Value)
                {
                    if (method.GetParameters().Length <= 0)
                    {
                        s += "      " + SystemInstanceVar(method.DeclaringType) + "." + method.Name + "();\n";
                    }
                    else if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(IEnv))
                    {
                        s += "      " + SystemInstanceVar(method.DeclaringType) + "." + method.Name + "(this);\n";
                    }
                    else
                    {
                        bool supplyEntity = false;
                        List<Type> components = new List<Type>();
                        foreach (ParameterInfo parameter in method.GetParameters())
                        {
                            if (parameter.ParameterType == typeof(Entity))
                            {
                                supplyEntity = true;
                            } else if (parameter.ParameterType == typeof(IEnv))
                            {
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

                        foreach (string world in ctx.worlds)
                        {
                            string entityVar = "entity" + nextVarId++;
                            s += "      foreach (var " + entityVar + " in " + FilterName(world, components) + ") {\n";
                            s += "        " + SystemInstanceVar(method.DeclaringType) + "." + method.Name + "(";
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

                                if (parameter.ParameterType == typeof(Entity))
                                {
                                    s += "new Entity(" + WorldVar(world) + ", " + entityVar + ")";
                                }
                                else if (parameter.ParameterType == typeof(IEnv))
                                {
                                    s += "this";
                                }
                                else
                                {
                                    Type componentType;
                                    if (parameter.ParameterType.IsByRef)
                                    {
                                        s += "ref ";
                                        componentType = parameter.ParameterType.GetElementType();
                                    }
                                    else
                                    {
                                        componentType = parameter.ParameterType;
                                    }

                                    s += PoolVar(world, componentType) + ".Get(" + entityVar + ")";
                                }
                            }

                            s += ");\n";
                            s += "      }\n";
                        }
                    }
                }

                s += "    };\n";
            }

            return s;
        }


        private static string GenerateMethods(Context ctx)
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
                    s += "    w.GetPool<T" + i + ">().Add(id) = c" + i + ";\n";
                }

                s += "    return new Entity(w, id);\n";
                s += "  }\n";
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

                s += "  public void Query<" + gsig + ">(SimpleCallback<" + gsig + "> callback)\n" + where + "  {\n";
                s += "    foreach (EcsWorld w in allWorlds) {\n";

                s += "      EcsFilter filter = w.Filter<T1>()";
                for (int i = 2; i <= componentCount; i++)
                {
                    s += ".Inc<T" + i + ">()";
                }

                s += ".End();\n";
                s += "      foreach (var id in filter) {\n";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"        if (!w.GetPool<T{i}>().Has(id)) continue;\n";
                }

                s += "        callback(";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ",";
                    s += "ref w.GetPool<T" + i + ">().Get(id)";
                }

                s += ");\n";
                s += "      }\n";

                s += "    }\n";
                s += "  }\n";
                s += "  public void Query<" + gsig + ">(EntityCallback<" + gsig + "> callback)\n" + where + "  {\n";
                s += "    foreach (EcsWorld w in allWorlds) {\n";

                s += "      EcsFilter filter = w.Filter<T1>()";
                for (int i = 2; i <= componentCount; i++)
                {
                    s += ".Inc<T" + i + ">()";
                }

                s += ".End();\n";
                s += "      foreach (var id in filter) {\n";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"        if (!w.GetPool<T{i}>().Has(id)) continue;\n";
                }

                s += "        callback(new Entity(w, id), ";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ",";
                    s += "ref w.GetPool<T" + i + ">().Get(id)";
                }

                s += ");\n";
                s += "      }\n";

                s += "    }\n";
                s += "  }\n";
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

                s += "  public bool Match<" + gsig + ">(Entity entity, SimpleCallback<" + gsig + "> callback)\n" + where + "  {\n";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"    if (!entity.Has<T{i}>()) return false;\n";
                }

                s += "    callback(";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ",";
                    s += "ref entity.Get<T" + i + ">()";
                }

                s += ");\n";
                s += "    return true;\n";

                s += "  }\n";

                s += "  public bool Match<" + gsig + ">(Entity entity, EntityCallback<" + gsig + "> callback)\n" + where + "  {\n";
                for (int i = 1; i <= componentCount; i++)
                {
                    s += $"    if (!entity.Has<T{i}>()) return false;\n";
                }

                s += "    callback(entity, ";
                for (int i = 1; i <= componentCount; i++)
                {
                    if (i > 1) s += ",";
                    s += "ref entity.Get<T" + i + ">()";
                }

                s += ");\n";
                s += "    return true;\n";

                s += "  }\n";
            }


            return s;
        }

        private static string GenerateNestedClasses(Context ctx)
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

        private static void Scan(Context ctx, IEnumerable<Assembly> assemblies)
        {
            ctx.systemsByPhase = new Dictionary<Type, List<MethodInfo>>();
            ctx.systemClasses = new List<Type>();

            HashSet<string> worlds = new HashSet<string>();
            worlds.Add("");

            foreach (Assembly assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    foreach (Exception loaderException in e.LoaderExceptions)
                    {
                        Debug.LogError(loaderException);
                    }

                    Debug.LogError(e);
                    continue;
                }

                foreach (Type type in types)
                {
                    if (type.GetCustomAttribute<EcsPhaseAttribute>() != null)
                    {
                        ctx.systemsByPhase[type] = new List<MethodInfo>();
                        ctx.phases.Add(type);
                    }

                    if (type.GetCustomAttribute<EcsSystemAttribute>() != null)
                    {
                        ctx.systemClasses.Add(type);

                        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            if (field.GetCustomAttribute<InjectAttribute>() != null)
                            {
                                ctx.injections.Add(new Injection
                                {
                                    field = field.Name,
                                    system = type,
                                    type = field.FieldType
                                });
                            }
                        }
                    }

                    EcsWorldAttribute ecsWorldAttribute = type.GetCustomAttribute<EcsWorldAttribute>();
                    if (ecsWorldAttribute != null)
                    {
                        worlds.Add(ecsWorldAttribute.name);
                    }
                }
            }

            ctx.worlds.AddRange(worlds.OrderBy(it => it));

            foreach (Type systemClass in ctx.systemClasses)
            {
                foreach (MethodInfo method in systemClass.GetMethods())
                {
                    foreach (Attribute attribute in method.GetCustomAttributes())
                    {
                        if (ctx.systemsByPhase.TryGetValue(attribute.GetType(), out var systemsForPhase))
                        {
                            systemsForPhase.Add(method);
                        }
                    }
                }
            }


            foreach (List<MethodInfo> methods in ctx.systemsByPhase.Values)
            {
                foreach (MethodInfo method in methods)
                {
                    List<Type> filter = new List<Type>();
                    foreach (ParameterInfo parameter in method.GetParameters())
                    {
                        if (parameter.ParameterType == typeof(Entity))
                        {
                            continue;
                        }
                        if (parameter.ParameterType == typeof(IEnv))
                        {
                            continue;
                        }

                        Type componentType = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
                        ctx.components.Add(componentType);
                        filter.Add(componentType);
                    }

                    if (filter.Any())
                    {
                        ctx.filters.Add(filter.OrderBy(x => x.Name).ToList());
                    }
                }
            }

            BusyEcs.SortSystems systemsOrder = BusyEcs.SystemsOrder;
            if (systemsOrder != null)
            {
                foreach (List<MethodInfo> systems in ctx.systemsByPhase.Values)
                {
                    MethodInfo[] temp = systems.ToArray();
                    systemsOrder(temp);
                    systems.Clear();
                    foreach (MethodInfo system in temp)
                    {
                        systems.Add(system);
                    }
                }
            }
        }
    }
}
#endif