namespace Kk.BusyEcs
{
    public interface IEcsContainer
    {
        void Execute<T>(T phase) where T : struct;
    }
}