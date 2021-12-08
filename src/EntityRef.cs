using System;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly struct EntityRef
    {
        private readonly EcsPackedEntityWithWorld _packed;

        public EntityRef(EcsPackedEntityWithWorld packed)
        {
            _packed = packed;
        }

        public Entity Deref()
        {
            if (!_packed.Unpack(out EcsWorld world, out int id))
            {
                throw new Exception("stale entity");
            }

            return new Entity(world, id);
        }

        public bool TryDeref(out Entity result)
        {
            if (!_packed.Unpack(out EcsWorld world, out int id))
            {
                result = default;
                return false;
            }

            result = new Entity(world, id);
            return true;
        }
    }
}