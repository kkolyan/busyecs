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
        
        public bool Match<T1>(SimpleCallback<T1> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2>(SimpleCallback<T1, T2> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2, T3>(SimpleCallback<T1, T2, T3> handler) { throw new NotImplementedException(); }
        public bool Match<T1>(EntityCallback<T1> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2>(EntityCallback<T1, T2> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2, T3>(EntityCallback<T1, T2, T3> handler) { throw new NotImplementedException(); }

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