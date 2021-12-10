namespace Kk.BusyEcs
{
    public interface IEnv
    {
        void Query<T1>(SimpleCallback<T1> handler);
        void Query<T1, T2>(SimpleCallback<T1, T2> handler);
        void Query<T1, T2, T3>(SimpleCallback<T1, T2, T3> handler);

        void Query<T1>(EntityCallback<T1> handler);
        void Query<T1, T2>(EntityCallback<T1, T2> handler);
        void Query<T1, T2, T3>(EntityCallback<T1, T2, T3> handler);

        Entity NewEntity<T1>(in T1 t1);
        Entity NewEntity<T1, T2>(in T1 t1, in T2 t2);
        Entity NewEntity<T1, T2, T3>(in T1 t1, in T2 t2, in T3 t3);
        Entity NewEntity<T1, T2, T3, T4>(in T1 t1, in T2 t2, in T3 t3, in T4 t4);
        Entity NewEntity<T1, T2, T3, T4, T5>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5);
        Entity NewEntity<T1, T2, T3, T4, T5, T6>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6);
        Entity NewEntity<T1, T2, T3, T4, T5, T6, T7>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6, in T7 t7);
        Entity NewEntity<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 t1, in T2 t2, in T3 t3, in T4 t4, in T5 t5, in T6 t6, in T7 t7, in T8 t8);
    }
}