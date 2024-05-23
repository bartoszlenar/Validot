namespace Validot.Validation.Scopes
{
    using System;
    using System.Collections.Generic;

    internal class DictionaryCommandScope<T, TKey, TValue> : CommandScope<T>
        where T : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public int ScopeId { get; set; }

        public Func<TKey, string> KeyStringifier { get; set; }

        protected override void RunDiscovery(IDiscoveryContext context)
        {
            context.EnterCollectionItemPath();

            context.EnterScope<TValue>(ScopeId);

            context.LeavePath();
        }

        protected override void RunValidation(T model, IValidationContext context)
        {
            foreach (var pair in model)
            {
                var keyRaw = KeyStringifier is null ? pair.Key as string : KeyStringifier(pair.Key);

                var keyNormalized = PathHelper.NormalizePath(keyRaw);

                context.EnterPath(keyNormalized);

                context.EnterScope(ScopeId, pair.Value);

                context.LeavePath();

                if (context.ShouldFallBack)
                {
                    break;
                }
            }
        }
    }
}