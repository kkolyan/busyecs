using System;
using System.Runtime.CompilerServices;
using Kk.BusyEcs.Internal;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    public struct EcsWorldRef : IEquatable<EcsWorldRef>
    {
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsWorldRef(EcsWorld world) {
            _index = Array.IndexOf(WorldsKeeper.worlds, world);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsWorld GetWorld()
        {
            return WorldsKeeper.worlds[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DelEntity(int entity)
        {
            WorldsKeeper.worlds[_index].DelEntity(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void GetComponents(int entity, ref object[] list)
        {
            WorldsKeeper.worlds[_index].GetComponents(entity, ref list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetComponentsCount(int entity)
        {
            return WorldsKeeper.worlds[_index].GetComponentsCount(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EcsPool<T> GetPool<T>() where T : struct
        {
            return PoolKeeper<T>.byWorld[_index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal EcsPackedEntityWithWorld PackEntityWithWorld(int entity)
        {
            return WorldsKeeper.worlds[_index].PackEntityWithWorld(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EcsWorldRef other)
        {
            return _index == other._index;
        }

        public override bool Equals(object obj)
        {
            return obj is EcsWorldRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _index;
        }
    }
}