using System;

namespace Kk.BusyEcs
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class EcsWorldAttribute : Attribute
    {
        public readonly string name;

        public EcsWorldAttribute(string name)
        {
            this.name = name;
        }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class EcsPhaseAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Class)]
    public class EcsSystemAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute { }
}