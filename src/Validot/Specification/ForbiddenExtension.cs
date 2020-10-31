namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class ForbiddenExtension
    {
        /// <summary>
        /// Forbids the current scope to have a value - only null is valid. In case of non-null value, an error is saved in the current scope.
        /// This is a presence command - must be at the beginning, error output can be altered by following parameter commands: WithMessage, WithExtraMessage, WithCode, WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IForbiddenOut<T> Forbidden<T>(this IForbiddenIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(ForbiddenCommand.Instance);
        }

        /// <inheritdoc cref="Forbidden{T}(Validot.Specification.IForbiddenIn{T})"/>
        public static IForbiddenOut<T?> Forbidden<T>(this IForbiddenIn<T?> @this)
            where T : struct
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T?>)@this).AddCommand(ForbiddenCommand.Instance);
        }
    }

    namespace Specification
    {
        public interface IForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithMessageForbiddenIn<T>,
            IWithExtraMessageForbiddenIn<T>,
            IWithCodeForbiddenIn<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IForbiddenIn<T>
        {
        }

        public partial interface ISpecificationIn<T> : IForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IForbiddenIn<T>, IForbiddenOut<T>
        {
        }
    }
}
