namespace Validot
{
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Specification;
    using Validot.Translations;

    public static class BaseCollectionRules
    {
        public static IRuleOut<TCollection> EmptyCollection<TCollection, TItem>(this IRuleIn<TCollection> @this)
            where TCollection : IEnumerable<TItem>
        {
            return @this.RuleTemplate(m => !m.Any(), MessageKey.Collections.EmptyCollection);
        }

        public static IRuleOut<TCollection> NotEmptyCollection<TCollection, TItem>(this IRuleIn<TCollection> @this)
            where TCollection : IEnumerable<TItem>
        {
            return @this.RuleTemplate(m => m.Any(), MessageKey.Collections.NotEmptyCollection);
        }

        public static IRuleOut<TCollection> ExactCollectionSize<TCollection, TItem>(this IRuleIn<TCollection> @this, int size)
            where TCollection : IEnumerable<TItem>
        {
            ThrowHelper.BelowZero(size, nameof(size));

            return @this.RuleTemplate(m => m.Count() == size, MessageKey.Collections.ExactCollectionSize, Arg.Number(nameof(size), size));
        }

        public static IRuleOut<TCollection> MaxCollectionSize<TCollection, TItem>(this IRuleIn<TCollection> @this, int max)
            where TCollection : IEnumerable<TItem>
        {
            ThrowHelper.BelowZero(max, nameof(max));

            return @this.RuleTemplate(m => m.Count() <= max, MessageKey.Collections.MaxCollectionSize, Arg.Number(nameof(max), max));
        }

        public static IRuleOut<TCollection> MinCollectionSize<TCollection, TItem>(this IRuleIn<TCollection> @this, int min)
            where TCollection : IEnumerable<TItem>
        {
            ThrowHelper.BelowZero(min, nameof(min));

            return @this.RuleTemplate(m => m.Count() >= min, MessageKey.Collections.MinCollectionSize, Arg.Number(nameof(min), min));
        }

        public static IRuleOut<TCollection> CollectionSizeBetween<TCollection, TItem>(this IRuleIn<TCollection> @this, int min, int max)
            where TCollection : IEnumerable<TItem>
        {
            ThrowHelper.BelowZero(min, nameof(min));
            ThrowHelper.BelowZero(max, nameof(max));

            ThrowHelper.InvalidRange(min, nameof(min), max, nameof(max));

            return @this.RuleTemplate(
                m =>
                {
                    var count = m.Count();

                    return count >= min && count <= max;
                },
                MessageKey.Collections.CollectionSizeBetween,
                Arg.Number(nameof(min), min),
                Arg.Number(nameof(max), max));
        }
    }
}
