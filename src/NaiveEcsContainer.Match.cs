namespace Kk.BusyEcs
{
    internal partial class NaiveEcsContainer
    {

        public bool Match<T1>(Entity entity, SimpleCallback<T1> callback) where T1 : struct => MatchInternal(entity, callback);
        public bool Match<T1>(Entity entity, EntityCallback<T1> callback) where T1 : struct => MatchInternal(entity, callback);

        public bool Match<T1, T2>(Entity entity, SimpleCallback<T1, T2> callback) where T1 : struct where T2 : struct => MatchInternal(entity, callback);
        public bool Match<T1, T2>(Entity entity, EntityCallback<T1, T2> callback) where T1 : struct where T2 : struct => MatchInternal(entity, callback);

        public bool Match<T1, T2, T3>(Entity entity, SimpleCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => MatchInternal(entity, callback);
        public bool Match<T1, T2, T3>(Entity entity, EntityCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => MatchInternal(entity, callback);

        public bool Match<T1, T2, T3, T4>(Entity entity, SimpleCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => MatchInternal(entity, callback);
        public bool Match<T1, T2, T3, T4>(Entity entity, EntityCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => MatchInternal(entity, callback);
    }
}