namespace Validot
{
    using System;
    using System.Collections.Generic;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsDictionaryExtension
    {
        /// <summary>
        /// Validates every value in the directory against the given specification.
        /// Each value's error output is saved under the path of this value's key in the dictionary.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="specification"><see cref="Specification{T}"/> for the collection's items.</param>
        /// <param name="keyStringifier">Function that creates a string version of the dictionary's keys, so they can be used as a path segment in the results.</param>
        /// <typeparam name="T">Type of the dictionary.</typeparam>
        /// <typeparam name="TKey">Type of the dictionary's key.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary's value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> AsDictionary<T, TKey, TValue>(this IRuleIn<T> @this, Specification<TValue> specification, Func<TKey, string> keyStringifier)
            where T : IEnumerable<KeyValuePair<TKey, TValue>>
        {
            ThrowHelper.NullArgument(@this, nameof(@this));
            ThrowHelper.NullArgument(keyStringifier, nameof(keyStringifier));

            return ((SpecificationApi<T>)@this).AddCommand(new AsDictionaryCommand<T, TKey, TValue>(specification, keyStringifier));
        }

        /// <inheritdoc cref="AsDictionary{T,TKey,TValue}"/>
        public static IRuleOut<IReadOnlyCollection<KeyValuePair<TKey, TValue>>> AsDictionary<TKey, TValue>(this IRuleIn<IReadOnlyCollection<KeyValuePair<TKey, TValue>>> @this, Specification<TValue> specification, Func<TKey, string> keyStringifier)
        {
            return @this.AsDictionary<IReadOnlyCollection<KeyValuePair<TKey, TValue>>, TKey, TValue>(specification, keyStringifier);
        }

        /// <inheritdoc cref="AsDictionary{T,TKey,TValue}"/>
        public static IRuleOut<Dictionary<TKey, TValue>> AsDictionary<TKey, TValue>(this IRuleIn<Dictionary<TKey, TValue>> @this, Specification<TValue> specification, Func<TKey, string> keyStringifier)
        {
            return @this.AsDictionary<Dictionary<TKey, TValue>, TKey, TValue>(specification, keyStringifier);
        }

        /// <inheritdoc cref="AsDictionary{T,TKey,TValue}"/>
        public static IRuleOut<IDictionary<TKey, TValue>> AsDictionary<TKey, TValue>(this IRuleIn<IDictionary<TKey, TValue>> @this, Specification<TValue> specification, Func<TKey, string> keyStringifier)
        {
            return @this.AsDictionary<IDictionary<TKey, TValue>, TKey, TValue>(specification, keyStringifier);
        }

        /// <inheritdoc cref="AsDictionary{T,TKey,TValue}"/>
        public static IRuleOut<IReadOnlyDictionary<TKey, TValue>> AsDictionary<TKey, TValue>(this IRuleIn<IReadOnlyDictionary<TKey, TValue>> @this, Specification<TValue> specification, Func<TKey, string> keyStringifier)
        {
            return @this.AsDictionary<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(specification, keyStringifier);
        }
    }
}