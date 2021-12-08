using System;
using Leopotam.EcsLite;

namespace Kk.BusyEcs
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class EcsArchetypeAttribute: Attribute
    {
        public readonly string name;

        public EcsArchetypeAttribute(string name = "") {
            this.name = name;
        }
    }
}