namespace Kk.BusyEcs
{
    public delegate void SimpleCallback<T1>(ref T1 t1);
    public delegate void SimpleCallback<T1, T2>(ref T1 t1, ref T2 t2);
    public delegate void SimpleCallback<T1, T2, T3>(ref T1 t1, ref T2 t2, ref T3 t3);
    public delegate void SimpleCallback<T1, T2, T3, T4>(ref T1 t1, ref T2 t2, ref T3 t3, ref T4 t4);
}