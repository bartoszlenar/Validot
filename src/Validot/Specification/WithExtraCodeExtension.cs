namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithExtraCodeExtension
    {
        public static IWithExtraCodeOut<T> WithExtraCode<T>(this IWithExtraCodeIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraCodeCommand(code));
        }

        public static IWithExtraCodeForbiddenOut<T> WithExtraCode<T>(this IWithExtraCodeForbiddenIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraCodeCommand(code));
        }
    }

    namespace Specification
    {
        public interface IWithExtraCodeOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithExtraCodeIn<T>
        {
        }

        public interface IWithExtraCodeForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithExtraCodeForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithExtraCodeIn<T>, IWithExtraCodeOut<T>, IWithExtraCodeForbiddenIn<T>, IWithExtraCodeForbiddenOut<T>
        {
        }
    }
}
