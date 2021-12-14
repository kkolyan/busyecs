namespace Kk.BusyEcs
{
    internal partial class NaiveEcsContainer
    {
        public Entity NewEntity<T1>(in T1 c1) where T1 : struct => NewEntityInternal(c1);
        public Entity NewEntity<T1, T2>(in T1 c1, in T2 c2) where T1 : struct where T2 : struct => NewEntityInternal(c1, c2);
        public Entity NewEntity<T1, T2, T3>(in T1 c1, in T2 c2, in T3 c3) where T1 : struct where T2 : struct where T3 : struct => NewEntityInternal(c1, c2, c3);
        public Entity NewEntity<T1, T2, T3, T4>(in T1 c1, in T2 c2, in T3 c3, in T4 c4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => NewEntityInternal(c1, c2, c3, c4);
        public Entity NewEntity<T1, T2, T3, T4, T5>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct => NewEntityInternal(c1, c2, c3, c4, c5);
        public Entity NewEntity<T1, T2, T3, T4, T5, T6>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct => NewEntityInternal(c1, c2, c3, c4, c5, c6);
        public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct => NewEntityInternal(c1, c2, c3, c4, c5, c6, c7);
        public Entity NewEntity<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct where T8 : struct => NewEntityInternal(c1, c2, c3, c4, c5, c6, c7, c8);
    }
}