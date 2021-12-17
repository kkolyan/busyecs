namespace Kk.BusyEcs
{
    public readonly partial struct EntityRef
    {

        public bool Match<T1>(SimpleCallback0<T1> callback) where T1 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1>(EntityCallback0<T1> callback) where T1 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1>(SimpleCallback1<T1> callback) where T1 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1>(EntityCallback1<T1> callback) where T1 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2>(SimpleCallback0<T1, T2> callback) where T1 : struct where T2 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2>(EntityCallback0<T1, T2> callback) where T1 : struct where T2 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2>(SimpleCallback1<T1, T2> callback) where T1 : struct where T2 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2>(EntityCallback1<T1, T2> callback) where T1 : struct where T2 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2>(SimpleCallback2<T1, T2> callback) where T1 : struct where T2 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2>(EntityCallback2<T1, T2> callback) where T1 : struct where T2 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3>(SimpleCallback0<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3>(EntityCallback0<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3>(SimpleCallback1<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3>(EntityCallback1<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3>(SimpleCallback2<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3>(EntityCallback2<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3>(SimpleCallback3<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3>(EntityCallback3<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3, T4>(SimpleCallback0<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3, T4>(EntityCallback0<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3, T4>(SimpleCallback1<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3, T4>(EntityCallback1<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3, T4>(SimpleCallback2<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3, T4>(EntityCallback2<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3, T4>(SimpleCallback3<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3, T4>(EntityCallback3<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);

        public bool Match<T1, T2, T3, T4>(SimpleCallback4<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);
        public bool Match<T1, T2, T3, T4>(EntityCallback4<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => Deref(out Entity entity) && entity.Match(callback);
    }
}