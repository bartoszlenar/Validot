namespace Validot.Validation.Scopes
{
    using System;

    internal class MemberCommandScope<T, TMember> : CommandScope<T>
    {
        public Func<T, TMember> GetMemberValue { get; set; }

        public int ScopeId { get; set; }

        protected override void RunDiscovery(IDiscoveryContext context)
        {
            context.EnterScope<TMember>(ScopeId);
        }

        protected override void RunValidation(T model, IValidationContext context)
        {
            var memberValue = GetMemberValue(model);

            context.EnterScope(ScopeId, memberValue);
        }
    }
}
