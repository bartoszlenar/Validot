namespace Validot
{
    using Validot.Specification;

    public static class ArrayRules
    {
        public static IRuleOut<TItem[]> EmptyCollection<TItem>(this IRuleIn<TItem[]> @this)
        {
            return @this.EmptyCollection<TItem[], TItem>();
        }

        public static IRuleOut<TItem[]> NotEmptyCollection<TItem>(this IRuleIn<TItem[]> @this)
        {
            return @this.NotEmptyCollection<TItem[], TItem>();
        }

        public static IRuleOut<TItem[]> ExactCollectionSize<TItem>(this IRuleIn<TItem[]> @this, int size)
        {
            return @this.ExactCollectionSize<TItem[], TItem>(size);
        }

        public static IRuleOut<TItem[]> MaxCollectionSize<TItem>(this IRuleIn<TItem[]> @this, int max)
        {
            return @this.MaxCollectionSize<TItem[], TItem>(max);
        }

        public static IRuleOut<TItem[]> MinCollectionSize<TItem>(this IRuleIn<TItem[]> @this, int min)
        {
            return @this.MinCollectionSize<TItem[], TItem>(min);
        }

        public static IRuleOut<TItem[]> CollectionSizeBetween<TItem>(this IRuleIn<TItem[]> @this, int min, int max)
        {
            return @this.CollectionSizeBetween<TItem[], TItem>(min, max);
        }
    }
}
