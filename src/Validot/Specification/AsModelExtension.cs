namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class AsModelExtension
    {
        /// <summary>
        /// Validates the current scope value against the specification. The error output is saved in the current scope.
        /// This is a scope command - its error output can be altered with any of the parameter commands (WithCondition, WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode).
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="specification">Specification for the current scope value.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRuleOut<T> AsModel<T>(this IRuleIn<T> @this, Specification<T> specification)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new AsModelCommand<T>(specification));
        }
    }
}
