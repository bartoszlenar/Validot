namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsTypeExtension
    {
        /// <summary>
        /// Casts the current scope value to another type and validates it against the specification. If the value can't be cast - nothing happens. The error output is saved in the current scope.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="specification">A specification for type <typeparamref name="TType"/> used to validate the value after casting.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <typeparam name="TType">Type that the value is cast to.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> AsType<T, TType>(this IRuleIn<T> @this, Specification<TType> specification)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new AsTypeCommand<T, TType>(specification));
        }
    }
}
