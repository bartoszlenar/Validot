namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;

    public static class IReadOnlyListRules
    {
        public static IRuleOut<IReadOnlyList<TItem>> EmptyCollection<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this)
        {
            return @this.EmptyCollection<IReadOnlyList<TItem>, TItem>();
        }

        public static IRuleOut<IReadOnlyList<TItem>> NotEmptyCollection<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this)
        {
            return @this.NotEmptyCollection<IReadOnlyList<TItem>, TItem>();
        }

        public static IRuleOut<IReadOnlyList<TItem>> ExactCollectionSize<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this, int size)
        {
            return @this.ExactCollectionSize<IReadOnlyList<TItem>, TItem>(size);
        }

        public static IRuleOut<IReadOnlyList<TItem>> MinCollectionSize<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this, int min)
        {
            return @this.MinCollectionSize<IReadOnlyList<TItem>, TItem>(min);
        }

        public static IRuleOut<IReadOnlyList<TItem>> MaxCollectionSize<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this, int max)
        {
            return @this.MaxCollectionSize<IReadOnlyList<TItem>, TItem>(max);
        }

        public static IRuleOut<IReadOnlyList<TItem>> CollectionSizeBetween<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this, int min, int max)
        {
            return @this.CollectionSizeBetween<IReadOnlyList<TItem>, TItem>(min, max);
        }
    }
}