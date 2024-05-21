namespace Validot
{
    using System.Collections.Generic;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsDictionaryWithStringKeyExtension
    {
        /// <summary>
        /// Validates every value in the directory against the given specification.
        /// Each value's error output is saved under the path of this value's key in the dictionary.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="specification"><see cref="Specification{T}"/> for the collection's items.</param>
        /// <typeparam name="T">Type of the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary's value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> AsDictionary<T, TValue>(this IRuleIn<T> @this, Specification<TValue> specification)
            where T : IEnumerable<KeyValuePair<string, TValue>>
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new AsDictionaryCommand<T, string, TValue>(specification, null));
        }

        /// <inheritdoc cref="AsDictionary{T,TValue}"/>
        public static IRuleOut<IReadOnlyCollection<KeyValuePair<string, TValue>>> AsDictionary<TValue>(this IRuleIn<IReadOnlyCollection<KeyValuePair<string, TValue>>> @this, Specification<TValue> specification)
        {
            return @this.AsDictionary<IReadOnlyCollection<KeyValuePair<string, TValue>>, TValue>(specification);
        }

        /// <inheritdoc cref="AsDictionary{T,TValue}"/>
        public static IRuleOut<Dictionary<string, TValue>> AsDictionary<TValue>(this IRuleIn<Dictionary<string, TValue>> @this, Specification<TValue> specification)
        {
            return @this.AsDictionary<Dictionary<string, TValue>, TValue>(specification);
        }

        /// <inheritdoc cref="AsDictionary{T,TValue}"/>
        public static IRuleOut<IDictionary<string, TValue>> AsDictionary<TValue>(this IRuleIn<IDictionary<string, TValue>> @this, Specification<TValue> specification)
        {
            return @this.AsDictionary<IDictionary<string, TValue>, TValue>(specification);
        }

        /// <inheritdoc cref="AsDictionary{T,TValue}"/>
        public static IRuleOut<IReadOnlyDictionary<string, TValue>> AsDictionary<TValue>(this IRuleIn<IReadOnlyDictionary<string, TValue>> @this, Specification<TValue> specification)
        {
            return @this.AsDictionary<IReadOnlyDictionary<string, TValue>, TValue>(specification);
        }
    }
}