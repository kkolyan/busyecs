using System;

namespace Kk.BusyEcs
{
    internal partial class NaiveEcsContainer
    {
        public Entity NewEntity<T1>(in T1 t1) => NewEntityInternal(t1);
        public Entity NewEntity<T1, T2>(in T1 t1, in T2 t2) => NewEntityInternal(t1, t2);
        public Entity NewEntity<T1, T2, T3>(in T1 t1, in T2 t2, in T3 t3) => NewEntityInternal(t1, t2, t3);
        public Entity NewEntity<T1, T2, T3, T4>(in T1 t1, in T2 t2, in T3 t3, in T4 t4) => NewEntityInternal(t1, t2, t3, t4);
        public Entity NewEntity<T1, T2, T3, T4, T5>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5) => NewEntityInternal(t1, t2, t3, t4, t5);
        public Entity NewEntity<T1, T2, T3, T4, T5, T6>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6) => NewEntityInternal(t1, t2, t3, t4, t5, t6);
        public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6, in T7 t7) => NewEntityInternal(t1, t2, t3, t4, t5, t6, t7);
        public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6, in T7 t7, in T8 t8) => NewEntityInternal(t1, t2, t3, t4, t5, t6, t7, t8);
    }
}