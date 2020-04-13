namespace Validot.Validation.Scopes
{
    using System.Collections.Generic;

    internal class CollectionCommandScope<T, TItem> : CommandScope<T>
        where T : IEnumerable<TItem>
    {
        public int ScopeId { get; set; }

        protected override void RunDiscovery(IDiscoveryContext context)
        {
            context.EnterCollectionItemPath();

            context.EnterScope<TItem>(ScopeId);

            context.LeavePath();
        }

        protected override void RunValidation(T model, IValidationContext context)
        {
            var i = 0;

            foreach (var item in model)
            {
                context.EnterCollectionItemPath(i);

                context.EnterScope(ScopeId, item);

                context.LeavePath();

                if (context.ShouldFallBack)
                {
                    break;
                }

                ++i;
            }
        }
    }
}
