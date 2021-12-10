using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Leopotam.EcsLite;
using UnityEngine;

namespace Kk.BusyEcs
{
    internal partial class NaiveEcsContainer : IEcsContainer, IEnv
    {
        private readonly Dictionary<Type, string> _worldRequirements = new Dictionary<Type, string>();
        private readonly Dictionary<Type, List<PhaseHandler>> _byPhase = new Dictionary<Type, List<PhaseHandler>>();
        private readonly EcsSystems _ecsSystems;

        internal NaiveEcsContainer(Dictionary<Type, object> services, List<Assembly> assemblies)
        {
            services[typeof(IEnv)] = this;
            try
            {
                List<Type> systemClasses = new List<Type>();
                HashSet<Type> phases = new HashSet<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly.GetName().Name.StartsWith("Microsoft"))
                    {
                        continue;
                    }

                    Debug.Log($"Scanning assembly: {assembly}");
                    try
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (type.GetCustomAttribute<EcsSystemClassAttribute>() != null)
                            {
                                systemClasses.Add(type);
                            }

                            EcsWorldAttribute ecsWorldAttribute = type.GetCustomAttribute<EcsWorldAttribute>();
                            if (ecsWorldAttribute != null)
                            {
                                _worldRequirements[type] = ecsWorldAttribute.name;
                            }

                            if (type.GetCustomAttribute<EcsPhaseAttribute>() != null)
                            {
                                phases.Add(type);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Failed to load assembly");
                        throw;
                    }
                }

                _ecsSystems = new EcsSystems(new EcsWorld());
                foreach (KeyValuePair<Type, string> worldRequirement in _worldRequirements)
                {
                    _ecsSystems.AddWorld(new EcsWorld(), worldRequirement.Value);
                }

                foreach (Type type in systemClasses)
                {
                    object systemInstance = Activator.CreateInstance(type);

                    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                    {
                        if (field.GetCustomAttribute<InjectAttribute>() != null)
                        {
                            if (!services.TryGetValue(field.FieldType, out var service))
                            {
                                throw new Exception($"cannot resolve dependency for field {field}");
                            }

                            field.SetValue(systemInstance, service);
                        }
                    }

                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                    {
                        EcsSystem ecsSystem = method.GetCustomAttribute<EcsSystem>();
                        if (ecsSystem != null)
                        {
                            Type phaseType = method.GetParameters()[0].ParameterType;

                            if (!_byPhase.TryGetValue(phaseType, out var handlersOfType))
                            {
                                handlersOfType = new List<PhaseHandler>();
                                _byPhase[phaseType] = handlersOfType;
                            }

                            if (phaseType.GetCustomAttribute<EcsPhaseAttribute>() == null)
                            {
                                throw new Exception($"malformed ES method: {method}. " +
                                                    $"first argument should be a struct attributed with [{nameof(EcsPhaseAttribute)}]");
                            }

                            if (method.GetParameters().Length == 1)
                            {
                                handlersOfType.Add(new PhaseHandler(
                                    HandlerName(method),
                                    o => { method.Invoke(systemInstance, new[] { o }); }
                                ));
                            }
                            else
                            {
                                handlersOfType.Add(new PhaseHandler(
                                        HandlerName(method),
                                        phase =>
                                        {
                                            ForEachInFilter(
                                                method,
                                                _ecsSystems,
                                                skipParams: 1,
                                                parameterValues =>
                                                {
                                                    parameterValues[0] = phase;
                                                    try
                                                    {
                                                        method.Invoke(systemInstance, parameterValues);
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Debug.Log(
                                                            $"failed to call {method} with parameters [{string.Join(", ", parameterValues.Select(it => it?.ToString() ?? "null"))}]");
                                                        throw;
                                                    }
                                                });
                                        }
                                    )
                                );
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                throw new Exception(string.Join("\n", e.LoaderExceptions.Select(it => it.ToString())));
            }

            foreach (KeyValuePair<Type, List<PhaseHandler>> phase in _byPhase)
            {
                Debug.Log($"Phase: {phase.Key.Name}");
                foreach (PhaseHandler handler in phase.Value)
                {
                    Debug.Log($"Phase handler: {handler.name}");
                }
            }
        }

        private static string HandlerName(MethodInfo m)
        {
            return $"{m.DeclaringType.Name} . {m.Name} ( {string.Join(", ", m.GetParameters().Select(it => it.ParameterType.Name))} )";
        }

        private struct PhaseHandler
        {
            public string name;
            public Action<object> action;

            public PhaseHandler(string name, Action<object> action)
            {
                this.name = name;
                this.action = action;
            }
        }

        private static void ForEachInFilter(MethodInfo method, EcsSystems ecsSystems, int skipParams, Action<object[]> callback)
        {
            int extraSkip = SupplyEntity(method, skipParams) ? 1 : 0;

            void WithWorld(EcsWorld world)
            {
                object filterMask = typeof(EcsWorld).GetMethod(nameof(EcsWorld.Filter))
                    .MakeGenericMethod(DropByRef(method.GetParameters()[skipParams + extraSkip].ParameterType))
                    .Invoke(world, Array.Empty<object>());

                for (var i = skipParams + 1 + extraSkip; i < method.GetParameters().Length; i++)
                {
                    ParameterInfo parameter = method.GetParameters()[i];
                    filterMask = typeof(EcsFilter.Mask).GetMethod(nameof(EcsFilter.Mask.Inc))
                        .MakeGenericMethod(DropByRef(parameter.ParameterType))
                        .Invoke(filterMask, Array.Empty<object>());
                }

                EcsFilter filter = (EcsFilter)typeof(EcsFilter.Mask).GetMethod(nameof(EcsFilter.Mask.End))
                    .Invoke(filterMask, new object[] { 512 });

                foreach (int entity in filter)
                {
                    ForEntity(method, world, skipParams, callback, entity);
                }
            }

            WithWorld(ecsSystems.GetWorld());
            foreach (EcsWorld world in ecsSystems.GetAllNamedWorlds().Values)
            {
                WithWorld(world);
            }
        }

        private static Type DropByRef(Type type)
        {
            if (type.IsByRef)
            {
                return type.GetElementType();
            }

            return type;
        }

        public static bool ForEntity(MethodInfo method,
            EcsWorld world,
            int skipParams,
            Action<object[]> callback,
            int entity)
        {
            var parameterValues = new object[method.GetParameters().Length];
            if (SupplyEntity(method, skipParams))
            {
                parameterValues[skipParams] = new Entity(world, entity);
                skipParams++;
            }

            for (var i = skipParams; i < method.GetParameters().Length; i++)
            {
                ParameterInfo parameter1 = method.GetParameters()[i];
                IEcsPool pool = (IEcsPool)typeof(EcsWorld)
                    .GetMethod(nameof(EcsWorld.GetPool))
                    .MakeGenericMethod(DropByRef(parameter1.ParameterType))
                    .Invoke(world, Array.Empty<object>());
                if (!pool.Has(entity))
                {
                    return false;
                }

                parameterValues[i] = pool.GetRaw(entity);
            }

            callback(parameterValues);

            for (var i = skipParams; i < method.GetParameters().Length; i++)
            {
                ParameterInfo parameter2 = method.GetParameters()[i];
                if (parameter2.ParameterType.IsByRef)
                {
                    IEcsPool pool = (IEcsPool)typeof(EcsWorld)
                        .GetMethod(nameof(EcsWorld.GetPool))
                        .MakeGenericMethod(DropByRef(parameter2.ParameterType))
                        .Invoke(world, Array.Empty<object>());
                    if (pool.Has(entity))
                    {
                        pool.SetRaw(entity, parameterValues[i]);
                    }
                }
            }

            return true;
        }

        private static bool SupplyEntity(MethodInfo method, int skipParams)
        {
            return skipParams >= 0 && method.GetParameters().Length > skipParams &&
                   method.GetParameters()[skipParams].ParameterType == typeof(Entity);
        }

        public void Execute<T>(T phase) where T : struct
        {
            if (_byPhase.TryGetValue(typeof(T), out var actions))
            {
                foreach (PhaseHandler action in actions)
                {
                    action.action(phase);
                }
            }
        }

        public EcsSystems GetWorlds()
        {
            return _ecsSystems;
        }

        private void QueryInternal(Delegate callback)
        {
            ForEachInFilter(
                callback.Method,
                _ecsSystems,
                skipParams: 0, callback:
                paramValues => callback.DynamicInvoke(paramValues)
            );
        }

        private Entity NewEntityInternal(params object[] componentValues)
        {
            string worldName = null;
            foreach (object componentValue in componentValues)
            {
                if (_worldRequirements.TryGetValue(componentValue.GetType(), out var w))
                {
                    if (worldName != null && worldName != w)
                    {
                        throw new Exception($"world resolution conflict: {worldName} and {w}");
                    }

                    if (w != "") // attribute value cannot be null
                    {
                        worldName = w;
                    }
                }
            }

            EcsWorld world = _ecsSystems.GetWorld(worldName);

            int entity = world.NewEntity();
            foreach (object componentValue in componentValues)
            {
                IEcsPool pool = (IEcsPool)typeof(EcsWorld)
                    .GetMethod(nameof(EcsWorld.GetPool))
                    .MakeGenericMethod(componentValue.GetType())
                    .Invoke(world, Array.Empty<object>());
                pool.AddRaw(entity, componentValue);
            }

            return new Entity(world, entity);
        }
    }
}