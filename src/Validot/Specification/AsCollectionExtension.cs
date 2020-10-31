namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsCollectionExtension
    {
        /// <summary>
        /// Validates every collection's item against the given specification.
        /// Each item's error output is saved under the path "#X", where X is the item's order within the collection.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="specification"><see cref="Specification{T}"/> for the collection's items.</param>
        /// <typeparam name="T">Type of the collection.</typeparam>
        /// <typeparam name="TItem">Type of the collection's item.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> AsCollection<T, TItem>(this IRuleIn<T> @this, Specification<TItem> specification)
            where T : IEnumerable<TItem>
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new AsCollectionCommand<T, TItem>(specification));
        }

        /// <inheritdoc cref="AsCollection{T,TItem}"/>
        public static IRuleOut<TItem[]> AsCollection<TItem>(this IRuleIn<TItem[]> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<TItem[], TItem>(specification);
        }

        /// <inheritdoc cref="AsCollection{T,TItem}"/>
        public static IRuleOut<IEnumerable<TItem>> AsCollection<TItem>(this IRuleIn<IEnumerable<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IEnumerable<TItem>, TItem>(specification);
        }

        /// <inheritdoc cref="AsCollection{T,TItem}"/>
        public static IRuleOut<ICollection<TItem>> AsCollection<TItem>(this IRuleIn<ICollection<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<ICollection<TItem>, TItem>(specification);
        }

        /// <inheritdoc cref="AsCollection{T,TItem}"/>
        public static IRuleOut<IReadOnlyCollection<TItem>> AsCollection<TItem>(this IRuleIn<IReadOnlyCollection<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IReadOnlyCollection<TItem>, TItem>(specification);
        }

        /// <inheritdoc cref="AsCollection{T,TItem}"/>
        public static IRuleOut<IList<TItem>> AsCollection<TItem>(this IRuleIn<IList<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IList<TItem>, TItem>(specification);
        }

        /// <inheritdoc cref="AsCollection{T,TItem}"/>
        public static IRuleOut<IReadOnlyList<TItem>> AsCollection<TItem>(this IRuleIn<IReadOnlyList<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<IReadOnlyList<TItem>, TItem>(specification);
        }

        /// <inheritdoc cref="AsCollection{T,TItem}"/>
        public static IRuleOut<List<TItem>> AsCollection<TItem>(this IRuleIn<List<TItem>> @this, Specification<TItem> specification)
        {
            return @this.AsCollection<List<TItem>, TItem>(specification);
        }
    }
}
