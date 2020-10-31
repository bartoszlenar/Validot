namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class OptionalExtension
    {
        /// <summary>
        /// Allows the current scope value to have null. In case of null, no error is saved.
        /// This is a presence command - must be at the beginning.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
        public static IOptionalOut<T> Optional<T>(this IOptionalIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(OptionalCommand.Instance);
        }

        /// <inheritdoc cref="Optional{T}(Validot.Specification.IOptionalIn{T})"/>
        public static IOptionalOut<T?> Optional<T>(this IOptionalIn<T?> @this)
            where T : struct
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T?>)@this).AddCommand(OptionalCommand.Instance);
        }
    }

    namespace Specification
    {
        public interface IOptionalOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>
        {
        }

        public interface IOptionalIn<T>
        {
        }

        public partial interface ISpecificationIn<T> : IOptionalIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IOptionalIn<T>, IOptionalOut<T>
        {
        }
    }
}
