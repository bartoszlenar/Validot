namespace Validot
{
    using System;

    using Validot.Specification;
    using Validot.Specification.Commands;

    public static class WithConditionExtension
    {
        /// <summary>
        /// Sets execution condition of the closest preceding scope command. If the condition is not met, the validation logic is not even executed.
        /// This is a parameter command - it can be followed by a new scope command or other parameter commands: WithPath, WithMessage, WithExtraMessage, WithCode, WithExtraCode.
        /// </summary>
        /// <param name="this">Fluent API builder - input.</param>
        /// <param name="executionCondition">Execution condition of the closest preceding scope command. A predicate that receives the current scope's value and determines whether the validation logic should be executed (returns true if yes, otherwise - false).</param>
        /// <typeparam name="T">Type of the current scope value.</typeparam>
        /// <returns>Fluent API builder - output.</returns>
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
            IRuleIn<T>,
            IWithPathIn<T>,
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
