namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;

    public static class IListRules
    {
        public static IRuleOut<IList<TItem>> EmptyCollection<TItem>(this IRuleIn<IList<TItem>> @this)
        {
            return @this.EmptyCollection<IList<TItem>, TItem>();
        }

        public static IRuleOut<IList<TItem>> NotEmptyCollection<TItem>(this IRuleIn<IList<TItem>> @this)
        {
            return @this.NotEmptyCollection<IList<TItem>, TItem>();
        }

        public static IRuleOut<IList<TItem>> ExactCollectionSize<TItem>(this IRuleIn<IList<TItem>> @this, int size)
        {
            return @this.ExactCollectionSize<IList<TItem>, TItem>(size);
        }

        public static IRuleOut<IList<TItem>> MinCollectionSize<TItem>(this IRuleIn<IList<TItem>> @this, int min)
        {
            return @this.MinCollectionSize<IList<TItem>, TItem>(min);
        }

        public static IRuleOut<IList<TItem>> MaxCollectionSize<TItem>(this IRuleIn<IList<TItem>> @this, int max)
        {
            return @this.MaxCollectionSize<IList<TItem>, TItem>(max);
        }

        public static IRuleOut<IList<TItem>> CollectionSizeBetween<TItem>(this IRuleIn<IList<TItem>> @this, int min, int max)
        {
            return @this.CollectionSizeBetween<IList<TItem>, TItem>(min, max);
        }
    }
}