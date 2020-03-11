namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WhenExtension
    {
        public static IWhenOut<T> When<T>(this IWhenIn<T> @this, Predicate<T> executionCondition)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WhenCommand<T>(executionCondition));
        }
    }

    namespace Specification
    {
        public interface IWhenOut<T> :
            ISpecificationOut<T>,
            IWithErrorClearedIn<T>,
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWhenIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWhenIn<T>, IWhenOut<T>
        {
        }
    }
}
