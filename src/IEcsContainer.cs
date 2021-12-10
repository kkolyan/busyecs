using System;

namespace Kk.BusyEcs
{
    public interface IEcsContainer : IEnv
    {
        void Execute<T>() where T : Attribute;
    }
}