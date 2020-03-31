namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class RequiredExtension
    {
        public static IRequiredOut<T> Required<T>(this IRequiredIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(RequiredCommand.Instance);
        }

        public static IRequiredOut<T?> Required<T>(this IRequiredIn<T?> @this)
            where T : struct
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T?>)@this).AddCommand(RequiredCommand.Instance);
        }

        public static IRequiredOut<T> NotNull<T>(this IRequiredIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(RequiredCommand.Instance);
        }

        public static IRequiredOut<T?> NotNull<T>(this IRequiredIn<T?> @this)
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
