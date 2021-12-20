using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Kk.BusyEcs.Internal;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public readonly partial struct Entity : IEquatable<Entity>
    {
        internal readonly int world;
        internal readonly int id;

        public override string ToString() {
            return $"Entity({world}:{id:x8}, [{string.Join(", ", Components.Select(it => it.GetType().Name))}])";
        }

        internal object[] Components
        {
            get
            {
                EcsWorld ecsWorld = GetWorld();
                object[] list = new object[ecsWorld.GetComponentsCount(id)];
                ecsWorld.GetComponents(id, ref list);
                return list;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity(int world, int id)
        {
            this.world = world;
            this.id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity(EcsWorld world, int id)
        {
            this.world = Array.IndexOf(WorldsKeeper.worlds, world);
            this.id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityRef AsRef()
        {
            return new EntityRef(WorldsKeeper.worlds[world].PackEntityWithWorld(id));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Add<T>() where T : struct
        {
            return ref PoolKeeper<T>.byWorld[world].Add(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Add<T>(in T initialState) where T : struct
        {
            PoolKeeper<T>.byWorld[world].Add(id) = initialState;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DelEntity()
        {
            WorldsKeeper.worlds[world].DelEntity(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Del<T>() where T : struct
        {
            PoolKeeper<T>.byWorld[world].Del(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get<T>() where T : struct
        {
            return ref PoolKeeper<T>.byWorld[world].Get(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has<T>() where T : struct
        {
            return PoolKeeper<T>.byWorld[world].Has(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity other)
        {
            return world == other.world && id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (world * 397) ^ id;
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
        public EcsWorld GetWorld()
        {
            return WorldsKeeper.worlds[world];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetInternalId()
        {
            return id;
        }
    }
}