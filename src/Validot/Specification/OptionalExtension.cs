namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class OptionalExtension
    {
        public static IOptionalOut<T> Optional<T>(this IOptionalIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(OptionalCommand.Instance);
        }

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
