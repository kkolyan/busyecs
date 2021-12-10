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
    }

    public delegate void SimpleCallback<T1>(ref T1 t1);
    public delegate void SimpleCallback<T1, T2>(ref T1 t1, ref T2 t2);
    public delegate void SimpleCallback<T1, T2, T3>(ref T1 t1, ref T2 t2, ref T3 t3);
    public delegate void SimpleCallback<T1, T2, T3, T4>(ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);

    public delegate void EntityCallback<T1>(Entity entity, ref T1 t1);
    public delegate void EntityCallback<T1, T2>(Entity entity, ref T1 t1, ref T2 t2);
    public delegate void EntityCallback<T1, T2, T3>(Entity entity, ref T1 t1, ref T2 t2, ref T3 t3);
    public delegate void EntityCallback<T1, T2, T3, T4>(Entity entity, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);
}