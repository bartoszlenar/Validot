namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;

    public static class IEnumerableRules
    {
        public static IRuleOut<IEnumerable<TItem>> EmptyCollection<TItem>(this IRuleIn<IEnumerable<TItem>> @this)
        {
            return @this.EmptyCollection<IEnumerable<TItem>, TItem>();
        }

        public static IRuleOut<IEnumerable<TItem>> NotEmptyCollection<TItem>(this IRuleIn<IEnumerable<TItem>> @this)
        {
            return @this.NotEmptyCollection<IEnumerable<TItem>, TItem>();
        }

        public static IRuleOut<IEnumerable<TItem>> ExactCollectionSize<TItem>(this IRuleIn<IEnumerable<TItem>> @this, int size)
        {
            return @this.ExactCollectionSize<IEnumerable<TItem>, TItem>(size);
        }

        public static IRuleOut<IEnumerable<TItem>> MinCollectionSize<TItem>(this IRuleIn<IEnumerable<TItem>> @this, int min)
        {
            return @this.MinCollectionSize<IEnumerable<TItem>, TItem>(min);
        }

        public static IRuleOut<IEnumerable<TItem>> MaxCollectionSize<TItem>(this IRuleIn<IEnumerable<TItem>> @this, int max)
        {
            return @this.MaxCollectionSize<IEnumerable<TItem>, TItem>(max);
        }

        public static IRuleOut<IEnumerable<TItem>> CollectionSizeBetween<TItem>(this IRuleIn<IEnumerable<TItem>> @this, int min, int max)
        {
            return @this.CollectionSizeBetween<IEnumerable<TItem>, TItem>(min, max);
        }
    }
}