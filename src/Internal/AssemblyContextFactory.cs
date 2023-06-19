using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Kk.BusyEcs.Internal
{
    internal static class AssemblyContextFactory
    {
        public static CodegenContext ScanAssemblies(IEnumerable<Assembly> assemblies)
        {
            CodegenContext ctx = new CodegenContext();
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

                    if (type.GetCustomAttribute<EcsSystemAttribute>() != null && !type.Name.EndsWith("_Optimized"))
                    {
                        ctx.systemClasses.Add(type);

                        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            if (field.GetCustomAttribute<InjectAttribute>() != null)
                            {
                                ctx.injections.Add(new CodegenContext.Injection
                                {
                                    field = field.Name,
                                    target = type,
                                    subject = field.FieldType
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
                        if (EcsContainerSourcesGenerator.IsEntity(parameter.ParameterType))
                        {
                            continue;
                        }

                        if (!parameter.ParameterType.IsByRef && !parameter.IsIn)
                        {
                            Debug.LogWarning($"parameter `{method.DeclaringType?.FullName}.{method.Name}.{parameter.Name}` is declared as simple by-value. To avoid mistakes, consider declaring it as `in` to protect from errors and allow more optimizations by compiler.\n  parameter: {parameter},\n  method: {method}, \n  class: ({method.DeclaringType})");
                        }

                        Type componentType = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType;
                        ctx.components.Add(componentType);
                        filter.Add(componentType);
                    }

                    if (filter.Any())
                    {
                        var filterKey = string.Join(", ", filter.OrderBy(x => x.Name));
                        if (!ctx.filters.ContainsKey(filterKey))
                        {
                            ctx.filters[filterKey] = filter.OrderBy(x => x.Name).ToList();
                        }
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

            return ctx;
        }
    }
}