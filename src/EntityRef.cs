using System;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly partial struct EntityRef
    {
        private readonly EcsPackedEntityWithWorld _packed;

        public EntityRef(EcsPackedEntityWithWorld packed)
        {
            _packed = packed;
        }
        
        public bool Deref(out Entity result)
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