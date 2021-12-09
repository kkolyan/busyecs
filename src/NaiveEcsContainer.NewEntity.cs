using System;

namespace Kk.BusyEcs
{
    internal partial class NaiveEcsContainer
    {
        public Entity NewEntity<T1>(in T1 t1) => NewEntityInternal(t1);
        public Entity NewEntity<T1, T2>(in T1 t1, in T2 t2) => NewEntityInternal(t1, t2);
        public Entity NewEntity<T1, T2, T3>(in T1 t1, in T2 t2, in T3 t3) => NewEntityInternal(t1, t2, t3);
    }
}