namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithErrorClearedExtension
    {
        public static IWithErrorClearedOut<T> WithErrorCleared<T>(this IWithErrorClearedIn<T> @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(WithErrorClearedCommand.Instance);
        }

        public static IWithErrorClearedForbiddenOut<T> WithErrorCleared<T>(this IWithErrorClearedForbiddenIn<T> @this)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(WithErrorClearedCommand.Instance);
        }
    }

    namespace Specification
    {
        public interface IWithErrorClearedOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithMessageIn<T>,
            IWithCodeIn<T>
        {
        }

        public interface IWithErrorClearedIn<T>
        {
        }

        public interface IWithErrorClearedForbiddenOut<T> :
            ISpecificationOut<T>,
            IWithMessageForbiddenIn<T>,
            IWithCodeForbiddenIn<T>
        {
        }

        public interface IWithErrorClearedForbiddenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithErrorClearedIn<T>, IWithErrorClearedOut<T>, IWithErrorClearedForbiddenIn<T>, IWithErrorClearedForbiddenOut<T>
        {
        }
    }
}
