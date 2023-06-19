namespace Kk.BusyEcs.Internal
{
    public class ParamType
    {
        public string name;
        public Type type;

        public enum Type
        {
            Simple,
            Ref,
            In,
        }

        public ParamType(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }
}