using System;
using Kk.BusyEcs.Internal;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public class EcsContainerBuilder
    {
        private EcsSystems _explicitWorlds;
        private readonly IConfigurableEcsContainer _container;

        public EcsContainerBuilder(EcsMetadata metadata)
        {
            _container = (IConfigurableEcsContainer)Activator.CreateInstance(metadata.ecsContainerClass);
        }

        /// <summary>
        /// register things to be injected with `[Inject]` attribute. `Kk.BusyEcs.IEnv` instance is available by default as service
        /// </summary>
        /// <param name="service"></param>
        /// <param name="overrideType"></param>
        /// <returns></returns>
        public EcsContainerBuilder AddInjectable(object service, Type overrideType = null)
        {
            _container.AddInjectable(service, overrideType);
            return this;
        }


        /// <summary>
        /// for LeoECS Lite plugins integration. BusyECS uses supplied EcsSystems instance ONLY as worlds container (invokes nothing but GetAllNamedWorlds, GetWorld, AddWorld).
        /// </summary>
        public EcsContainerBuilder Integrate(EcsSystems ecsSystems)
        {
            _explicitWorlds = ecsSystems;
            return this;
        }

        public IEcsContainer Create()
        {
            _container.Init(_explicitWorlds ?? new EcsSystems(new EcsWorld()));
            return _container;
        }
    }
}