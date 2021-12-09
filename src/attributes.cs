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


    [AttributeUsage(AttributeTargets.Struct)]
    public class EcsPhaseAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Class)]
    public class EcsSystemClassAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute { }


    [AttributeUsage(AttributeTargets.Method)]
    public class EcsSystem : Attribute { }
}