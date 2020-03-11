namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithCodeExtension
    {
        public static IWithCodeOut<T> WithCode<T>(this IWithCodeIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithCodeCommand(code));
        }

        public static IWithCodeForbiddenOut<T> WithCode<T>(this IWithCodeForbiddenIn<T> @this, string code)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithCodeCommand(code));
        }
    }

    namespace Specification
    {
        public interface IWithCodeOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithCodeIn<T>
        {
        }

        public interface IWithCodeForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithCodeForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithCodeIn<T>, IWithCodeOut<T>, IWithCodeForbiddenIn<T>, IWithCodeForbiddenOut<T>
        {
        }
    }
}
