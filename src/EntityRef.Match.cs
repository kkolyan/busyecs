using System;

namespace Kk.BusyEcs
{
    public readonly partial struct EntityRef
    {
        public bool Match<T1>(SimpleCallback<T1> handler) => MatchInternal(handler);
        public bool Match<T1, T2>(SimpleCallback<T1, T2> handler) => MatchInternal(handler);
        public bool Match<T1, T2, T3>(SimpleCallback<T1, T2, T3> handler) => MatchInternal(handler);
        public bool Match<T1>(EntityCallback<T1> handler) => MatchInternal(handler);
        public bool Match<T1, T2>(EntityCallback<T1, T2> handler) => MatchInternal(handler);
        public bool Match<T1, T2, T3>(EntityCallback<T1, T2, T3> handler) => MatchInternal(handler);
    }
}