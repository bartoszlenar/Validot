namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class ForbiddenExtension
    {
        public static IForbiddenOut<T> Forbidden<T>(this IForbiddenIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(ForbiddenCommand.Instance);
        }

        public static IForbiddenOut<T> Null<T>(this IForbiddenIn<T> @this)
            where T : class
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(ForbiddenCommand.Instance);
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
