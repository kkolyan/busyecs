using System;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly struct Entity : IEquatable<Entity>
    {
        private readonly EcsWorld _world;
        private readonly int _id;

        public Entity(EcsWorld world, int id)
        {
            _world = world;
            _id = id;
        }
        
        public bool Match<T1>(SimpleCallback<T1> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2>(SimpleCallback<T1, T2> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2, T3>(SimpleCallback<T1, T2, T3> handler) { throw new NotImplementedException(); }
        public bool Match<T1>(EntityCallback<T1> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2>(EntityCallback<T1, T2> handler) { throw new NotImplementedException(); }
        public bool Match<T1, T2, T3>(EntityCallback<T1, T2, T3> handler) { throw new NotImplementedException(); }

        public EntityRef AsRef()
        {
            return new EntityRef(_world.PackEntityWithWorld(_id));
        }

        public ref T Add<T>() where T : struct
        {
            return ref _world.GetPool<T>().Add(_id);
        }

        public void DelEntity()
        {
            _world.DelEntity(_id);
        }

        public void Del<T>() where T : struct
        {
            _world.GetPool<T>().Del(_id);
        }

        public ref T Get<T>() where T : struct
        {
            return ref _world.GetPool<T>().Get(_id);
        }

        public bool Has<T>() where T : struct
        {
            return _world.GetPool<T>().Has(_id);
        }
        
        public bool Equals(Entity other)
        {
            return Equals(_world, other._world) && _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_world != null ? _world.GetHashCode() : 0) * 397) ^ _id;
            }
        }

        public static bool operator ==(Entity a, Entity b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }
    }
}