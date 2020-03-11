namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsCollectionExtension
    {
        public static IRuleOut<T> AsCollection<T, TItem>(this IRuleIn<T> @this, Specification<TItem> specification)
            where T : IEnumerable<TItem>
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new AsCollectionCommand<T, TItem>(specification));
        }

        public static IRuleOut<TItem[]> AsCollection<TItem>(this IRuleIn<TItem[]> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<TItem[], TItem>(specification);
        }

        public static IRuleOut<IEnumerable<TItem>> AsCollection<TItem>(this IRuleIn<IEnumerable<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IEnumerable<TItem>, TItem>(specification);
        }

        public static IRuleOut<ICollection<TItem>> AsCollection<TItem>(this IRuleIn<ICollection<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<ICollection<TItem>, TItem>(specification);
        }

        public static IRuleOut<IReadOnlyCollection<TItem>> AsCollection<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IReadOnlyCollection<TItem>, TItem>(specification);
        }

        public static IRuleOut<IList<TItem>> AsCollection<TItem>(this IRuleIn<IList<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IList<TItem>, TItem>(specification);
        }

        public static IRuleOut<IReadOnlyList<TItem>> AsCollection<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IReadOnlyList<TItem>, TItem>(specification);
        }

        public static IRuleOut<List<TItem>> AsCollection<TItem>(this IRuleIn<List<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<List<TItem>, TItem>(specification);
        }
    }
}
