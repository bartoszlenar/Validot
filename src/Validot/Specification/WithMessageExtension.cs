namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithMessageExtension
    {
        public static IWithMessageOut<T> WithMessage<T>(this IWithMessageIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithMessageCommand(message));
        }

        public static IWithMessageForbiddenOut<T> WithMessage<T>(this IWithMessageForbiddenIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithMessageCommand(message));
        }
    }

    namespace Specification
    {
        public interface IWithMessageOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraMessageIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithMessageIn<T>
        {
        }

        public interface IWithMessageForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraMessageForbiddenIn<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithMessageForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithMessageIn<T>, IWithMessageOut<T>, IWithMessageForbiddenIn<T>, IWithMessageForbiddenOut<T>
        {
        }
    }
}
