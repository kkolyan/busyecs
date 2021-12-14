namespace Kk.BusyEcs
{
    public interface IEnv
    {

        bool Match<T1>(Entity entity, SimpleCallback<T1> callback) where T1 : struct;
        bool Match<T1>(Entity entity, EntityCallback<T1> callback) where T1 : struct;
        void Query<T1>(SimpleCallback<T1> callback) where T1 : struct;
        void Query<T1>(EntityCallback<T1> callback) where T1 : struct;

        bool Match<T1, T2>(Entity entity, SimpleCallback<T1, T2> callback) where T1 : struct where T2 : struct;
        bool Match<T1, T2>(Entity entity, EntityCallback<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(SimpleCallback<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(EntityCallback<T1, T2> callback) where T1 : struct where T2 : struct;

        bool Match<T1, T2, T3>(Entity entity, SimpleCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        bool Match<T1, T2, T3>(Entity entity, EntityCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(SimpleCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(EntityCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;

        bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(SimpleCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(EntityCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

        Entity NewEntity<T1>(in T1 c1) where T1 : struct;
        Entity NewEntity<T1, T2>(in T1 c1, in T2 c2) where T1 : struct where T2 : struct;
        Entity NewEntity<T1, T2, T3>(in T1 c1, in T2 c2, in T3 c3) where T1 : struct where T2 : struct where T3 : struct;
        Entity NewEntity<T1, T2, T3, T4>(in T1 c1, in T2 c2, in T3 c3, in T4 c4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5, T6>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5, T6, T7>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct where T8 : struct;
    }
    

    public delegate void SimpleCallback<T1>(ref T1 c1);
    public delegate void EntityCallback<T1>(Entity entity, ref T1 c1);

    public delegate void SimpleCallback<T1, T2>(ref T1 c1, ref T2 c2);
    public delegate void EntityCallback<T1, T2>(Entity entity, ref T1 c1, ref T2 c2);

    public delegate void SimpleCallback<T1, T2, T3>(ref T1 c1, ref T2 c2, ref T3 c3);
    public delegate void EntityCallback<T1, T2, T3>(Entity entity, ref T1 c1, ref T2 c2, ref T3 c3);

    public delegate void SimpleCallback<T1, T2, T3, T4>(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4);
    public delegate void EntityCallback<T1, T2, T3, T4>(Entity entity, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4);
}