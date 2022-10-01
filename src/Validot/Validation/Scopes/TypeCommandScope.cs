namespace Validot.Validation.Scopes
{
    internal class TypeCommandScope<T, TType> : CommandScope<T>
    {
        public int ScopeId { get; set; }

        protected override void RunDiscovery(IDiscoveryContext context)
        {
            context.EnterScope<TType>(ScopeId);
        }

        protected override void RunValidation(T model, IValidationContext context)
        {
            if (model is TType typedValue)
            {
                context.EnterScope(ScopeId, typedValue);
            }
        }
    }
}
