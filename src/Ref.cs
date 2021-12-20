using Kk.BusyEcs.Internal;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly struct Ref<T> where T : struct
    {
        private readonly EcsPool<T> _pool;
        private readonly int _entity;

        public Ref(Entity entity)
        {
            _pool = PoolKeeper<T>.byWorld[entity.world];
            _entity = entity.id;
        }

        public ref T Get()
        {
            return ref _pool.Get(_entity);
        }

        public override string ToString() {
            return $"Ref<{typeof(T).Name}>({_entity:x8})";
        }
    }
}