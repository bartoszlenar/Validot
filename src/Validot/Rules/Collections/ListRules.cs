namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;

    public static class ListRules
    {
        public static IRuleOut<List<TItem>> EmptyCollection<TItem>(this IRuleIn<List<TItem>> @this)
        {
            return @this.EmptyCollection<List<TItem>, TItem>();
        }

        public static IRuleOut<List<TItem>> NotEmptyCollection<TItem>(this IRuleIn<List<TItem>> @this)
        {
            return @this.NotEmptyCollection<List<TItem>, TItem>();
        }

        public static IRuleOut<List<TItem>> ExactCollectionSize<TItem>(this IRuleIn<List<TItem>> @this, int size)
        {
            return @this.ExactCollectionSize<List<TItem>, TItem>(size);
        }

        public static IRuleOut<List<TItem>> MinCollectionSize<TItem>(this IRuleIn<List<TItem>> @this, int min)
        {
            return @this.MinCollectionSize<List<TItem>, TItem>(min);
        }

        public static IRuleOut<List<TItem>> MaxCollectionSize<TItem>(this IRuleIn<List<TItem>> @this, int max)
        {
            return @this.MaxCollectionSize<List<TItem>, TItem>(max);
        }

        public static IRuleOut<List<TItem>> CollectionSizeBetween<TItem>(this IRuleIn<List<TItem>> @this, int min, int max)
        {
            return @this.CollectionSizeBetween<List<TItem>, TItem>(min, max);
        }
    }
}