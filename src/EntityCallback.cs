namespace Kk.BusyEcs
{
    public delegate void EntityCallback<T1>(Entity entity, ref T1 t1);
    public delegate void EntityCallback<T1, T2>(Entity entity, ref T1 t1, ref T2 t2);
    public delegate void EntityCallback<T1, T2, T3>(Entity entity, ref T1 t1, ref T2 t2, ref T3 t3);
    public delegate void EntityCallback<T1, T2, T3, T4>(Entity entity, ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);
}