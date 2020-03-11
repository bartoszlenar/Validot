namespace Validot.Validation.Scopes
{
    using System.Collections.Generic;
    using System.Linq;

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
            var items = (model as IReadOnlyList<TItem>) ?? model.ToList();

            for (var i = 0; i < items.Count; ++i)
            {
                context.EnterCollectionItemPath(i);

                context.EnterScope(ScopeId, items[i]);

                context.LeavePath();

                if (context.ShouldFallBack)
                {
                    break;
                }
            }
        }
    }
}
