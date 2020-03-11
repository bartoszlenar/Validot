namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;

    public static class IReadOnlyCollectionRules
    {
        public static IRuleOut<IReadOnlyCollection<TItem>> EmptyCollection<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this)
        {
            return @this.EmptyCollection<IReadOnlyCollection<TItem>, TItem>();
        }

        public static IRuleOut<IReadOnlyCollection<TItem>> NotEmptyCollection<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this)
        {
            return @this.NotEmptyCollection<IReadOnlyCollection<TItem>, TItem>();
        }

        public static IRuleOut<IReadOnlyCollection<TItem>> ExactCollectionSize<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this, int size)
        {
            return @this.ExactCollectionSize<IReadOnlyCollection<TItem>, TItem>(size);
        }

        public static IRuleOut<IReadOnlyCollection<TItem>> MaxCollectionSize<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this, int max)
        {
            return @this.MaxCollectionSize<IReadOnlyCollection<TItem>, TItem>(max);
        }

        public static IRuleOut<IReadOnlyCollection<TItem>> MinCollectionSize<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this, int min)
        {
            return @this.MinCollectionSize<IReadOnlyCollection<TItem>, TItem>(min);
        }

        public static IRuleOut<IReadOnlyCollection<TItem>> CollectionSizeBetween<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this, int min, int max)
        {
            return @this.CollectionSizeBetween<IReadOnlyCollection<TItem>, TItem>(min, max);
        }
    }
}
