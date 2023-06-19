using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kk.BusyEcs.Internal
{
    internal class InitialPassResult
    {
        public Dictionary<CsSimpleName, List<CsTypeId>> knownStructs = new Dictionary<CsSimpleName, List<CsTypeId>>();
        public List<ClassDeclarationSyntax> systems = new List<ClassDeclarationSyntax>();
        public List<CsTypeId> phases = new List<CsTypeId>();
        public HashSet<string> extraWorldNames = new HashSet<string>();
        public Dictionary<CsTypeId, string> worldNameByComponent = new Dictionary<CsTypeId, string>();
    }
}