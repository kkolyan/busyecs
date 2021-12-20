namespace Kk.BusyEcs
{
    public interface IEnv
    {

        bool Match<T1>(Entity entity, out Ref<T1> c1) where T1 : struct;

        bool Match<T1>(Entity entity, SimpleCallback0<T1> callback) where T1 : struct;
        bool Match<T1>(Entity entity, EntityCallback0<T1> callback) where T1 : struct;
        void Query<T1>(SimpleCallback0<T1> callback) where T1 : struct;
        void Query<T1>(EntityCallback0<T1> callback) where T1 : struct;

        bool Match<T1>(Entity entity, SimpleCallback1<T1> callback) where T1 : struct;
        bool Match<T1>(Entity entity, EntityCallback1<T1> callback) where T1 : struct;
        void Query<T1>(SimpleCallback1<T1> callback) where T1 : struct;
        void Query<T1>(EntityCallback1<T1> callback) where T1 : struct;

        bool Match<T1, T2>(Entity entity, out Ref<T1> c1, out Ref<T2> c2) where T1 : struct where T2 : struct;

        bool Match<T1, T2>(Entity entity, SimpleCallback0<T1, T2> callback) where T1 : struct where T2 : struct;
        bool Match<T1, T2>(Entity entity, EntityCallback0<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(SimpleCallback0<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(EntityCallback0<T1, T2> callback) where T1 : struct where T2 : struct;

        bool Match<T1, T2>(Entity entity, SimpleCallback1<T1, T2> callback) where T1 : struct where T2 : struct;
        bool Match<T1, T2>(Entity entity, EntityCallback1<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(SimpleCallback1<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(EntityCallback1<T1, T2> callback) where T1 : struct where T2 : struct;

        bool Match<T1, T2>(Entity entity, SimpleCallback2<T1, T2> callback) where T1 : struct where T2 : struct;
        bool Match<T1, T2>(Entity entity, EntityCallback2<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(SimpleCallback2<T1, T2> callback) where T1 : struct where T2 : struct;
        void Query<T1, T2>(EntityCallback2<T1, T2> callback) where T1 : struct where T2 : struct;

        bool Match<T1, T2, T3>(Entity entity, out Ref<T1> c1, out Ref<T2> c2, out Ref<T3> c3) where T1 : struct where T2 : struct where T3 : struct;

        bool Match<T1, T2, T3>(Entity entity, SimpleCallback0<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        bool Match<T1, T2, T3>(Entity entity, EntityCallback0<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(SimpleCallback0<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(EntityCallback0<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;

        bool Match<T1, T2, T3>(Entity entity, SimpleCallback1<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        bool Match<T1, T2, T3>(Entity entity, EntityCallback1<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(SimpleCallback1<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(EntityCallback1<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;

        bool Match<T1, T2, T3>(Entity entity, SimpleCallback2<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        bool Match<T1, T2, T3>(Entity entity, EntityCallback2<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(SimpleCallback2<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(EntityCallback2<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;

        bool Match<T1, T2, T3>(Entity entity, SimpleCallback3<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        bool Match<T1, T2, T3>(Entity entity, EntityCallback3<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(SimpleCallback3<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;
        void Query<T1, T2, T3>(EntityCallback3<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct;

        bool Match<T1, T2, T3, T4>(Entity entity, out Ref<T1> c1, out Ref<T2> c2, out Ref<T3> c3, out Ref<T4> c4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

        bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback0<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback0<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(SimpleCallback0<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(EntityCallback0<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

        bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback1<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback1<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(SimpleCallback1<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(EntityCallback1<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

        bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback2<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback2<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(SimpleCallback2<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(EntityCallback2<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

        bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback3<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback3<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(SimpleCallback3<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(EntityCallback3<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

        bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback4<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback4<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(SimpleCallback4<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        void Query<T1, T2, T3, T4>(EntityCallback4<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;

        Entity NewEntity<T1>(in T1 c1) where T1 : struct;
        Entity NewEntity<T1, T2>(in T1 c1, in T2 c2) where T1 : struct where T2 : struct;
        Entity NewEntity<T1, T2, T3>(in T1 c1, in T2 c2, in T3 c3) where T1 : struct where T2 : struct where T3 : struct;
        Entity NewEntity<T1, T2, T3, T4>(in T1 c1, in T2 c2, in T3 c3, in T4 c4) where T1 : struct where T2 : struct where T3 : struct where T4 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5, T6>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5, T6, T7>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct;
        Entity NewEntity<T1, T2, T3, T4, T5, T6, T7, T8>(in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8) where T1 : struct where T2 : struct where T3 : struct where T4 : struct where T5 : struct where T6 : struct where T7 : struct where T8 : struct;
    }
    

    public delegate void SimpleCallback0<T1>(in T1 c1);
    public delegate void EntityCallback0<T1>(Entity entity, in T1 c1);

    public delegate void SimpleCallback1<T1>(ref T1 c1);
    public delegate void EntityCallback1<T1>(Entity entity, ref T1 c1);

    public delegate void SimpleCallback0<T1, T2>(in T1 c1, in T2 c2);
    public delegate void EntityCallback0<T1, T2>(Entity entity, in T1 c1, in T2 c2);

    public delegate void SimpleCallback1<T1, T2>(ref T1 c1, in T2 c2);
    public delegate void EntityCallback1<T1, T2>(Entity entity, ref T1 c1, in T2 c2);

    public delegate void SimpleCallback2<T1, T2>(ref T1 c1, ref T2 c2);
    public delegate void EntityCallback2<T1, T2>(Entity entity, ref T1 c1, ref T2 c2);

    public delegate void SimpleCallback0<T1, T2, T3>(in T1 c1, in T2 c2, in T3 c3);
    public delegate void EntityCallback0<T1, T2, T3>(Entity entity, in T1 c1, in T2 c2, in T3 c3);

    public delegate void SimpleCallback1<T1, T2, T3>(ref T1 c1, in T2 c2, in T3 c3);
    public delegate void EntityCallback1<T1, T2, T3>(Entity entity, ref T1 c1, in T2 c2, in T3 c3);

    public delegate void SimpleCallback2<T1, T2, T3>(ref T1 c1, ref T2 c2, in T3 c3);
    public delegate void EntityCallback2<T1, T2, T3>(Entity entity, ref T1 c1, ref T2 c2, in T3 c3);

    public delegate void SimpleCallback3<T1, T2, T3>(ref T1 c1, ref T2 c2, ref T3 c3);
    public delegate void EntityCallback3<T1, T2, T3>(Entity entity, ref T1 c1, ref T2 c2, ref T3 c3);

    public delegate void SimpleCallback0<T1, T2, T3, T4>(in T1 c1, in T2 c2, in T3 c3, in T4 c4);
    public delegate void EntityCallback0<T1, T2, T3, T4>(Entity entity, in T1 c1, in T2 c2, in T3 c3, in T4 c4);

    public delegate void SimpleCallback1<T1, T2, T3, T4>(ref T1 c1, in T2 c2, in T3 c3, in T4 c4);
    public delegate void EntityCallback1<T1, T2, T3, T4>(Entity entity, ref T1 c1, in T2 c2, in T3 c3, in T4 c4);

    public delegate void SimpleCallback2<T1, T2, T3, T4>(ref T1 c1, ref T2 c2, in T3 c3, in T4 c4);
    public delegate void EntityCallback2<T1, T2, T3, T4>(Entity entity, ref T1 c1, ref T2 c2, in T3 c3, in T4 c4);

    public delegate void SimpleCallback3<T1, T2, T3, T4>(ref T1 c1, ref T2 c2, ref T3 c3, in T4 c4);
    public delegate void EntityCallback3<T1, T2, T3, T4>(Entity entity, ref T1 c1, ref T2 c2, ref T3 c3, in T4 c4);

    public delegate void SimpleCallback4<T1, T2, T3, T4>(ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4);
    public delegate void EntityCallback4<T1, T2, T3, T4>(Entity entity, ref T1 c1, ref T2 c2, ref T3 c3, ref T4 c4);
}