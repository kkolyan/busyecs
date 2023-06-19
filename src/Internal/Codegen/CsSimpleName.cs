using System;

namespace Kk.BusyEcs.Internal
{
    public readonly struct CsSimpleName : IEquatable<CsSimpleName>
    {
        public readonly string name;

        public CsSimpleName(string name) {
            this.name = name;
        }

        public bool Equals(CsSimpleName other)
        {
            return name == other.name;
        }

        public override bool Equals(object obj)
        {
            return obj is CsSimpleName other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        public static bool operator ==(CsSimpleName left, CsSimpleName right) {
            return left.Equals(right);
        }

        public static bool operator !=(CsSimpleName left, CsSimpleName right) {
            return !left.Equals(right);
        }
    }
}