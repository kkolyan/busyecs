namespace Kk.BusyEcs
{
    public readonly partial struct Entity
    {
        // assigned from generated ECS container
        internal static IEnv env;

        public bool Match<T1>(SimpleCallback<T1> callback) where T1 : struct => env.Match(this, callback);
        public bool Match<T1>(EntityCallback<T1> callback) where T1 : struct => env.Match(this, callback);

        public bool Match<T1, T2>(SimpleCallback<T1, T2> callback) where T1 : struct where T2 : struct => env.Match(this, callback);
        public bool Match<T1, T2>(EntityCallback<T1, T2> callback) where T1 : struct where T2 : struct => env.Match(this, callback);

        public bool Match<T1, T2, T3>(SimpleCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => env.Match(this, callback);
        public bool Match<T1, T2, T3>(EntityCallback<T1, T2, T3> callback) where T1 : struct where T2 : struct where T3 : struct => env.Match(this, callback);

        public bool Match<T1, T2, T3, T4>(SimpleCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => env.Match(this, callback);
        public bool Match<T1, T2, T3, T4>(EntityCallback<T1, T2, T3, T4> callback) where T1 : struct where T2 : struct where T3 : struct where T4 : struct => env.Match(this, callback);
    }
}