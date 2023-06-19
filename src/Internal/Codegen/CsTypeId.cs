using System;

namespace Kk.BusyEcs.Internal
{
    public readonly struct CsTypeId : IEquatable<CsTypeId>
    {
        public readonly string name;

        public readonly CsNamespaceId ns;

        public CsTypeId(Type type) : this(type.Name, new CsNamespaceId(type.Namespace)) { }

        public CsTypeId(string name, CsNamespaceId ns)
        {
            this.name = name;
            this.ns = ns;
        }

        public bool Equals(CsTypeId other)
        {
            return name == other.name && ns.Equals(other.ns);
        }

        public override bool Equals(object obj)
        {
            return obj is CsTypeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((name != null ? name.GetHashCode() : 0) * 397) ^ ns.GetHashCode();
            }
        }

        public static bool operator ==(CsTypeId left, CsTypeId right) {
            return left.Equals(right);
        }

        public static bool operator !=(CsTypeId left, CsTypeId right) {
            return !left.Equals(right);
        }
    }
}