using Leopotam.EcsLite;

namespace Kk.BusyEcs.Internal
{
    public static class PoolKeeper<T> where T : struct
    {
        public static EcsPool<T>[] byWorld;

        static PoolKeeper()
        {
            byWorld = new EcsPool<T>[WorldsKeeper.worlds.Length];
            for (int i = 0; i < WorldsKeeper.worlds.Length; i++)
            {
                byWorld[i] = WorldsKeeper.worlds[i].GetPool<T>();
            }
        }
    }
}