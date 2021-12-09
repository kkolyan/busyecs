using System;
using System.Collections.Generic;

namespace Kk.BusyEcs
{
    public class EsContainerBuilder
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public EsContainerBuilder Injectable(object service, Type overrideType = null)
        {
            _services[overrideType ?? service.GetType()] = service;
            return this;
        }

        public IEcsContainer End()
        {
            return new NaiveEcsContainer(_services);
        }
    }
}