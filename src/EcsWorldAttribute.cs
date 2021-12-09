using System;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class EcsWorldAttribute: Attribute
    {
        public readonly string name;

        public EcsWorldAttribute(string name) {
            this.name = name;
        }
    }
}