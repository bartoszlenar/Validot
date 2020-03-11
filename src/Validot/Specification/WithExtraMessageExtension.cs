namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithExtraMessageExtension
    {
        public static IWithExtraMessageOut<T> WithExtraMessage<T>(this IWithExtraMessageIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraMessageCommand(message));
        }

        public static IWithExtraMessageForbiddenOut<T> WithExtraMessage<T>(this IWithExtraMessageForbiddenIn<T> @this, string message)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithExtraMessageCommand(message));
        }
    }

    namespace Specification
    {
        public interface IWithExtraMessageOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithExtraMessageIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithExtraMessageIn<T>
        {
        }

        public interface IWithExtraMessageForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithExtraMessageForbiddenIn<T>,
            IWithExtraCodeForbiddenIn<T>
        {
        }

        public interface IWithExtraMessageForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithExtraMessageIn<T>, IWithExtraMessageOut<T>, IWithExtraMessageForbiddenIn<T>, IWithExtraMessageForbiddenOut<T>
        {
        }
    }
}
