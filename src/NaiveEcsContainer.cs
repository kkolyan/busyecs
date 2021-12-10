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
        private readonly Dictionary<Type, string> _worldRequirements;
        private readonly Dictionary<Type, List<PhaseHandler>> _byPhase = new Dictionary<Type, List<PhaseHandler>>();
        private readonly EcsSystems _ecsSystems;

        internal NaiveEcsContainer(Dictionary<Type, object> services, List<Type> systemClasses, EcsSystems ecsSystems, Dictionary<Type, string> worldRequirements)
        {
            _worldRequirements = worldRequirements;
            _ecsSystems = ecsSystems ?? new EcsSystems(new EcsWorld());
            services[typeof(IEnv)] = this;
            try
            {
                foreach (KeyValuePair<Type, string> worldRequirement in _worldRequirements)
                {
                    if (_ecsSystems.GetWorld(worldRequirement.Value) == null)
                    {
                        _ecsSystems.AddWorld(new EcsWorld(), worldRequirement.Value);
                    }
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

                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                    {
                        foreach (Attribute attribute in method.GetCustomAttributes())
                        {
                            Type phaseType = attribute.GetType();

                            if (phaseType.GetCustomAttribute<EcsPhaseAttribute>() == null)
                            {
                                Debug.Log($"ignoring attribute {phaseType}, because it's not attributed with [EcsPhase]. method: {HandlerName(method)}");
                                continue;
                            }

                            if (!_byPhase.TryGetValue(phaseType, out var handlersOfType))
                            {
                                handlersOfType = new List<PhaseHandler>();
                                _byPhase[phaseType] = handlersOfType;
                            }

                            if (method.GetParameters().Length <= 0)
                            {
                                handlersOfType.Add(new PhaseHandler(
                                    HandlerName(method),
                                    () => { method.Invoke(systemInstance, Array.Empty<object>()); }
                                ));
                            }
                            else
                            {
                                handlersOfType.Add(new PhaseHandler(
                                        HandlerName(method),
                                        () =>
                                        {
                                            ForEachInFilter(
                                                method,
                                                _ecsSystems,
                                                parameterValues =>
                                                {
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
                    Debug.Log($"{phase.Key.Name}: {handler.name}");
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
            public Action action;

            public PhaseHandler(string name, Action action)
            {
                this.name = name;
                this.action = action;
            }
        }

        private static void ForEachInFilter(MethodInfo method, EcsSystems ecsSystems, Action<object[]> callback)
        {
            int extraSkip = SupplyEntity(method) ? 1 : 0;
            
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length - extraSkip <= 0)
            {
                throw new Exception("this method shouldn't be called for method without filters");
            }

            void WithWorld(EcsWorld world)
            {
                object filterMask = typeof(EcsWorld).GetMethod(nameof(EcsWorld.Filter))
                    .MakeGenericMethod(DropByRef(parameters[extraSkip].ParameterType))
                    .Invoke(world, Array.Empty<object>());

                for (var i = 1 + extraSkip; i < parameters.Length; i++)
                {
                    ParameterInfo parameter = parameters[i];
                    filterMask = typeof(EcsFilter.Mask).GetMethod(nameof(EcsFilter.Mask.Inc))
                        .MakeGenericMethod(DropByRef(parameter.ParameterType))
                        .Invoke(filterMask, Array.Empty<object>());
                }

                EcsFilter filter = (EcsFilter)typeof(EcsFilter.Mask).GetMethod(nameof(EcsFilter.Mask.End))
                    .Invoke(filterMask, new object[] { 512 });

                foreach (int entity in filter)
                {
                    ForEntity(method, world, callback, entity);
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
            Action<object[]> callback,
            int entity)
        {
            int skipParams = 0;
            var parameterValues = new object[method.GetParameters().Length];
            if (SupplyEntity(method))
            {
                parameterValues[0] = new Entity(world, entity);
                skipParams = 1;
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

        private static bool SupplyEntity(MethodInfo method)
        {
            return method.GetParameters().Length > 0 && method.GetParameters()[0].ParameterType == typeof(Entity);
        }

        public void Execute<T>() where T : Attribute
        {
            if (_byPhase.TryGetValue(typeof(T), out var actions))
            {
                foreach (PhaseHandler action in actions)
                {
                    action.action();
                }
            }
        }

        private void QueryInternal(Delegate callback)
        {
            ForEachInFilter(
                callback.Method,
                _ecsSystems, callback:
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