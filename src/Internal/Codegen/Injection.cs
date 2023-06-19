namespace Kk.BusyEcs.Internal
{
    internal readonly struct Injection
    {
        public readonly CsTypeId target;
        public readonly string fieldName;
        public readonly CsTypeId subject;

        public Injection(CsTypeId target, string fieldName, CsTypeId subject)
        {
            this.target = target;
            this.fieldName = fieldName;
            this.subject = subject;
        }
    }
}