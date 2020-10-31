namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsNullableExtension
    {
        /// <summary>
        /// Validates the current scope nullable value against the specification. The error output is saved in the current scope.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="specification">Specification for the value type <see cref="T"/>, wrapped in the nullable type.</param>
        /// <typeparam name="T">The value type wrapped in the nullable type.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T?> AsNullable<T>(this IRuleIn<T?> @this, Specification<T> specification)
            where T : struct
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T?>)@this).AddCommand(new AsNullableCommand<T>(specification));
        }
    }
}
