namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithConditionExtension
    {
        public static IWithConditionOut<T> WithCondition<T>(this IWithConditionIn<T> @this, Predicate<T> executionCondition)
        {
            ThrowHelper.NullArgument(@this, nameof(@this));

            return ((SpecificationApi<T>)@this).AddCommand(new WithConditionCommand<T>(executionCondition));
        }
    }

    namespace Specification
    {
        public interface IWithConditionOut<T> :
            ISpecificationOut<T>,
            IWithErrorClearedIn<T>,
            IWithMessageIn<T>,
            IWithExtraMessageIn<T>,
            IWithCodeIn<T>,
            IWithExtraCodeIn<T>
        {
        }

        public interface IWithConditionIn<T>
        {
        }

        internal partial class SpecificationApi<T> : IWithConditionIn<T>, IWithConditionOut<T>
        {
        }
    }
}
