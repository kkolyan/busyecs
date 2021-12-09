using System;
using System.Collections.Generic;
using System.Reflection;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    internal partial class NaiveEcsContainer : IEcsContainer, IEnv
    {
        private readonly Dictionary<Type, string> _worldRequirements = new Dictionary<Type, string>();
        private readonly Dictionary<Type, List<Action<object>>> _byPhase = new Dictionary<Type, List<Action<object>>>();
        private readonly EcsSystems _ecsSystems;

        private struct Preserve { }

        internal NaiveEcsContainer(Dictionary<Type, object> services)
        {
            List<Type> systemClasses = new List<Type>();
            HashSet<Type> phases = new HashSet<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
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

            _ecsSystems = new EcsSystems(new EcsWorld());

            foreach (Type type in systemClasses)
            {
                object systemInstance = Activator.CreateInstance(type);

                foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (field.GetCustomAttribute<InjectAttribute>() != null)
                    {
                        field.SetValue(systemInstance, services[field.FieldType]);
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
                            handlersOfType = new List<Action<object>>();
                            _byPhase[phaseType] = handlersOfType;
                        }

                        if (phaseType.GetCustomAttribute<EcsPhaseAttribute>() == null)
                        {
                            throw new Exception($"malformed ES method: {method}. " +
                                                $"first argument should be a struct attributed with [{nameof(EcsPhaseAttribute)}]");
                        }

                        Action<object> action = CreateFilterAction(
                            method, _ecsSystems,
                            skipParams: 1,
                            parameterValues => method.Invoke(systemInstance, parameterValues)
                        );

                        handlersOfType.Add(action);
                    }
                }
            }
        }

        private static Action<object> CreateFilterAction(MethodInfo method, EcsSystems ecsSystems, int skipParams, Action<object[]> callback)
        {
            void WithWorld(EcsWorld world)
            {
                object filterMask = typeof(EcsWorld).GetMethod(nameof(EcsWorld.Filter))
                    .MakeGenericMethod(method.GetParameters()[skipParams + 1].ParameterType)
                    .Invoke(world, Array.Empty<object>());
                for (var i = skipParams + 1; i < method.GetParameters().Length; i++)
                {
                    ParameterInfo parameter = method.GetParameters()[i];
                    filterMask = typeof(EcsFilter.Mask).GetMethod(nameof(EcsFilter.Mask.Inc))
                        .MakeGenericMethod(parameter.ParameterType)
                        .Invoke(filterMask, Array.Empty<object>());
                }

                EcsFilter filter = (EcsFilter)typeof(EcsFilter.Mask).GetMethod(nameof(EcsFilter.Mask.End))
                    .Invoke(filterMask, Array.Empty<object>());

                foreach (int entity in filter)
                {
                    ActOnEntity(method, world, skipParams, callback, entity);
                }
            }

            return o =>
            {
                WithWorld(ecsSystems.GetWorld());
                foreach (EcsWorld world in ecsSystems.GetAllNamedWorlds().Values)
                {
                    WithWorld(world);
                }
            };
        }

        public static bool ActOnEntity(MethodInfo method,
            EcsWorld world,
            int skipParams,
            Action<object[]> callback,
            int entity)
        {
            bool supplyEntity = method.GetParameters().Length > 1 && method.GetParameters()[1].ParameterType == typeof(Entity);

            EcsPool<Preserve> preservePool = world.GetPool<Preserve>();

            var parameterValues = new object[method.GetParameters().Length - skipParams];
            if (supplyEntity)
            {
                parameterValues[0] = new Entity(world, entity);
            }

            for (var i = skipParams + (supplyEntity ? 1 : 0); i < method.GetParameters().Length; i++)
            {
                ParameterInfo parameter1 = method.GetParameters()[i];
                IEcsPool pool = (IEcsPool)typeof(EcsWorld)
                    .GetMethod(nameof(EcsWorld.GetPool))
                    .MakeGenericMethod(parameter1.ParameterType)
                    .Invoke(world, Array.Empty<object>());
                if (!pool.Has(entity))
                {
                    return false;
                }

                parameterValues[i - skipParams] = pool.GetRaw(entity);
            }

            callback(parameterValues);

            for (var i = skipParams + (supplyEntity ? 1 : 0); i < method.GetParameters().Length; i++)
            {
                ParameterInfo parameter2 = method.GetParameters()[i];
                if (parameter2.ParameterType.IsByRef)
                {
                    IEcsPool pool = (IEcsPool)typeof(EcsWorld)
                        .GetMethod(nameof(EcsWorld.GetPool))
                        .MakeGenericMethod(parameter2.ParameterType)
                        .Invoke(world, Array.Empty<object>());

                    preservePool.Add(entity);
                    pool.Del(entity);
                    pool.AddRaw(entity, parameterValues[i - skipParams]);
                    preservePool.Del(entity);
                }
            }

            return true;
        }

        public void Execute<T>(T phase) where T : struct
        {
            foreach (Action<object> action in _byPhase[typeof(T)])
            {
                action(phase);
            }
        }

        private void QueryInternal(Delegate callback)
        {
            CreateFilterAction(
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
                IEcsPool pool = world.GetPoolByType(componentValue.GetType());
                pool.AddRaw(entity, componentValue);
            }

            return new Entity(world, entity);
        }
    }
}