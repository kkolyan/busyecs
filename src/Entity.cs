using System;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly partial struct Entity : IEquatable<Entity>
    {
        internal readonly EcsWorld world;
        internal readonly int id;

        internal object[] Components
        {
            get
            {
                object[] list = new object[world.GetComponentsCount(id)];
                world.GetComponents(id, ref list);
                return list;
            }
        }

        public Entity(EcsWorld world, int id)
        {
            this.world = world;
            this.id = id;
        }

        public EntityRef AsRef()
        {
            return new EntityRef(world.PackEntityWithWorld(id));
        }

        public ref T Add<T>() where T : struct
        {
            return ref world.GetPool<T>().Add(id);
        }

        public Entity Add<T>(in T initialState) where T : struct
        {
            world.GetPool<T>().Add(id) = initialState;
            return this;
        }

        public void DelEntity()
        {
            world.DelEntity(id);
        }

        public void Del<T>() where T : struct
        {
            world.GetPool<T>().Del(id);
        }

        public ref T Get<T>() where T : struct
        {
            return ref world.GetPool<T>().Get(id);
        }

        public bool Has<T>() where T : struct
        {
            return world.GetPool<T>().Has(id);
        }
        
        public bool Equals(Entity other)
        {
            return Equals(world, other.world) && id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((world != null ? world.GetHashCode() : 0) * 397) ^ id;
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

        public void GetRaw(out EcsWorld world, out int entity)
        {
            world = this.world;
            entity = id;
        }
    }
}