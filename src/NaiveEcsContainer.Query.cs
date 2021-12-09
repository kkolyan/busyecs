using System;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    internal partial class NaiveEcsContainer
    {
        public void Query<T1>(SimpleCallback<T1> handler) => QueryInternal(handler);
        public void Query<T1, T2>(SimpleCallback<T1, T2> handler) => QueryInternal(handler);
        public void Query<T1, T2, T3>(SimpleCallback<T1, T2, T3> handler) => QueryInternal(handler);
        
        public void Query<T1>(EntityCallback<T1> handler) => QueryInternal(handler);
        public void Query<T1, T2>(EntityCallback<T1, T2> handler) => QueryInternal(handler);
        public void Query<T1, T2, T3>(EntityCallback<T1, T2, T3> handler) => QueryInternal(handler);
    }
}