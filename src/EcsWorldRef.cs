using System;
using System.Runtime.CompilerServices;
using Kk.BusyEcs.Internal;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public struct EcsWorldRef : IEquatable<EcsWorldRef>
    {
        internal readonly int index;

        public override string ToString() {
            return $"{nameof(EcsWorldRef)}({index})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsWorldRef(EcsWorld world) {
            index = Array.IndexOf(WorldsKeeper.worlds, world);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsWorld GetWorld()
        {
            return WorldsKeeper.worlds[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DelEntity(int entity)
        {
            WorldsKeeper.worlds[index].DelEntity(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void GetComponents(int entity, ref object[] list)
        {
            WorldsKeeper.worlds[index].GetComponents(entity, ref list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetComponentsCount(int entity)
        {
            return WorldsKeeper.worlds[index].GetComponentsCount(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EcsPool<T> GetPool<T>() where T : struct
        {
            return PoolKeeper<T>.byWorld[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EcsPackedEntityWithWorld PackEntityWithWorld(int entity)
        {
            return WorldsKeeper.worlds[index].PackEntityWithWorld(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EcsWorldRef other)
        {
            return index == other.index;
        }

        public override bool Equals(object obj)
        {
            return obj is EcsWorldRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return index;
        }
    }
}