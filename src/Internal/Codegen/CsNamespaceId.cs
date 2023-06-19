using System;

namespace Kk.BusyEcs.Internal
{
    public readonly struct CsNamespaceId : IEquatable<CsNamespaceId>
    {
        public readonly string name;

        public CsNamespaceId(string name) {
            this.name = name;
        }

        public bool Equals(CsNamespaceId other)
        {
            return name == other.name;
        }

        public override bool Equals(object obj)
        {
            return obj is CsNamespaceId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        public static bool operator ==(CsNamespaceId left, CsNamespaceId right) {
            return left.Equals(right);
        }

        public static bool operator !=(CsNamespaceId left, CsNamespaceId right) {
            return !left.Equals(right);
        }
    }
}