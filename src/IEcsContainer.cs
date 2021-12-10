using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public interface IEcsContainer : IEnv
    {
        void Execute<T>(T phase) where T : struct;
        EcsSystems GetWorlds();
    }
}