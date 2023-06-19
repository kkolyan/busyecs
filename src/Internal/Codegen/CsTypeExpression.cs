using System.Collections.Generic;

namespace Kk.BusyEcs.Internal
{
    public readonly struct CsTypeExpression
    {
        public readonly CsTypeId qualifiedName;
        public readonly string simpleName;
        public readonly List<CsNamespaceId> possibleNamespaces;
        public readonly Dictionary<string, CsTypeId> aliases;

        public CsTypeExpression(CsTypeId qualifiedName) : this() {
            this.qualifiedName = qualifiedName;
        }

        public CsTypeExpression(string simpleName, List<CsNamespaceId> possibleNamespaces, Dictionary<string, CsTypeId> aliases) : this()
        {
            this.simpleName = simpleName;
            this.possibleNamespaces = possibleNamespaces;
            this.aliases = aliases;
        }
    }
}