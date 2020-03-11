namespace Validot.Validation.Scopes
{
    internal class ModelCommandScope<T> : CommandScope<T>
    {
        public int ScopeId { get; set; }

        protected override void RunDiscovery(IDiscoveryContext context)
        {
            context.EnterScope<T>(ScopeId);
        }

        protected override void RunValidation(T model, IValidationContext context)
        {
            context.EnterScope(ScopeId, model);
        }
    }
}
