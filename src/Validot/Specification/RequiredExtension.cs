namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class RequiredExtension
    {
        /// <summary>
        /// Forbids the current scope value to have null. In case of null, an error is saved in the current scope.
        /// All values are required by default - this command allows to modify the error output in case of null value.
        /// This is a presence command - must be at the beginning, error output can be altered by following parameter commands: WithMessage, WithExtraMessage, WithCode, WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IRequiredOut<T> Required<T>(this IRequiredIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(RequiredCommand.Instance);
        }

        /// <inheritdoc cref="Required{T}(Validot.Specification.IRequiredIn{T})"/>
        public static IRequiredOut<T?> Required<T>(this IRequiredIn<T?> @this)
            where T : struct
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T?>)@this).AddCommand(RequiredCommand.Instance);
        }
    }

    namespace Specification
    {
        public interface IRequiredOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IRequiredIn<T>
        {
        }

        public partial interface ISpecificationIn<T> : IRequiredIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IRequiredIn<T>, IRequiredOut<T>
        {
        }
    }
}
