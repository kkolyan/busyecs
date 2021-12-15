using System;

namespace Kk.BusyEcs.Internal
{
    public interface IConfigurableEcsContainer : IEcsContainer
    {
        void AddInjectable(object injectable, Type overrideType = null);
        void Init(Leopotam.EcsLite.EcsSystems systems);
    }
}