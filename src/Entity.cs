using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly partial struct Entity : IEquatable<Entity>
    {
        internal readonly EcsWorldRef world;
        internal readonly int id;

        public override string ToString() {
            return $"Entity({world.index}:{id}, [{string.Join(", ", Components.Select(it => it.GetType().Name))}])";
        }

        internal object[] Components
        {
            get
            {
                object[] list = new object[world.GetComponentsCount(id)];
                world.GetComponents(id, ref list);
                return list;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity(EcsWorld world, int id)
        {
            this.world = new EcsWorldRef(world);
            this.id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityRef AsRef()
        {
            return new EntityRef(world.PackEntityWithWorld(id));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Add<T>() where T : struct
        {
            return ref world.GetPool<T>().Add(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Add<T>(in T initialState) where T : struct
        {
            world.GetPool<T>().Add(id) = initialState;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DelEntity()
        {
            world.DelEntity(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del<T>() where T : struct
        {
            world.GetPool<T>().Del(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() where T : struct
        {
            return ref world.GetPool<T>().Get(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : struct
        {
            return world.GetPool<T>().Has(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity other)
        {
            return world.Equals(other.world) && id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (world.GetHashCode() * 397) ^ id;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Entity a, Entity b)
        {
            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsWorldRef GetWorldRef()
        {
            return world;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetInternalId()
        {
            return id;
        }
    }
}