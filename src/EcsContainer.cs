using System;

namespace Kk.BusyEcs
{
    public class EcsContainer
    {
        internal EcsContainer()
        {
            throw new NotImplementedException();
        }

        public void Execute<T>(T phase) where T : struct
        {
            throw new NotImplementedException();
        }
        
        public class Builder
        {
            public Builder Injectable(object service, Type overrideType = null)
            {
                throw new NotImplementedException();
            }
            
            public EcsContainer End()
            {
                throw new NotImplementedException();
            }
        }
    }
}