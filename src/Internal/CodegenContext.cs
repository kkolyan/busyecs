using System.Collections.Generic;
using System.Reflection;

namespace Kk.BusyEcs.Internal
{
    internal class CodegenContext
    {
        public HashSet<string> usings = new HashSet<string>();
        public List<Injection> injections = new List<Injection>();
        public List<CsTypeId> phases = new List<CsTypeId>();
        public Dictionary<CsTypeId, List<MethodInfo>> systemsByPhase = new Dictionary<CsTypeId, List<MethodInfo>>();
        public List<CsTypeId> systemClasses = new List<CsTypeId>();
        public HashSet<CsTypeId> components = new HashSet<CsTypeId>();
        public Dictionary<string, List<CsTypeId>> filters = new Dictionary<string, List<CsTypeId>>();
        public List<string> worlds = new List<string>();

        public class Injection
        {
            public CsTypeId target;
            public CsTypeId subject;
            public string field;

            public Injection(CsTypeId target, CsTypeId subject, string field)
            {
                this.target = target;
                this.subject = subject;
                this.field = field;
            }
        }

        internal CodegenContext() { }
    }
}