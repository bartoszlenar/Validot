namespace Validot.Validation.Scopes
{
    internal class NullableCommandScope<T> : CommandScope<T?>
        where T : struct
    {
        public int ScopeId { get; set; }

        protected override void RunDiscovery(IDiscoveryContext context)
        {
            context.EnterScope<T>(ScopeId);
        }

        protected override void RunValidation(T? model, IValidationContext context)
        {
            if (model.HasValue)
            {
                context.EnterScope(ScopeId, model.Value);
            }
        }
    }
}
