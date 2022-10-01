namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsConvertedExtension
    {
        /// <summary>
        /// Converts the current scope value and validates it against the specification. The error output is saved in the current scope.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="convert">A conversion function that takes the current scope value and outputs the new value.</param>
        /// <param name="specification">A specification for type <typeparamref name="TTarget"/> used to validate the converted value.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <typeparam name="TTarget">Type of the converted value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> AsConverted<T, TTarget>(this IRuleIn<T> @this, Converter<T, TTarget> convert, Specification<TTarget> specification)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            ThrowHelper.NullArgument(convert, nameof(convert));

            return ((SpecificationApi<T>)@this).AddCommand(new AsConvertedCommand<T, TTarget>(convert, specification));
        }
    }
}
