namespace Validot.Validation.Scopes
{
    using System;

    internal class ConvertedCommandScope<T, TTarget> : CommandScope<T>
    {
        public Converter<T, TTarget> Converter { get; set; }

        public int ScopeId { get; set; }

        protected override void RunDiscovery(IDiscoveryContext context)
        {
            context.EnterScope<TTarget>(ScopeId);
        }

        protected override void RunValidation(T model, IValidationContext context)
        {
            var convertedValue = Converter(model);

            context.EnterScope(ScopeId, convertedValue);
        }
    }
}