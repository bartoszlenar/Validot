namespace Validot
{
    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithNameExtension
    {
        public static IWithNameOut<T> WithName<T>(this IWithNameIn<T> @this, string name)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithNameCommand(name));
        }
    }

    namespace Specification
    {
        public interface IWithNameOut<T> :
            ISpecificationOut<T>,
            IRuleIn<T>,
            IWithErrorClearedIn<T>,
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithNameIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithNameIn<T>, IWithNameOut<T>
        {
        }
    }
}
